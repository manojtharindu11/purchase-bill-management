using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PurchaseBillManagement.Api.Data;
using PurchaseBillManagement.Api.DTOs.Auth;
using PurchaseBillManagement.Api.DTOs.External;
using PurchaseBillManagement.Api.DTOs.Locations;
using PurchaseBillManagement.Api.Models;
using PurchaseBillManagement.Api.Services.Exceptions;

namespace PurchaseBillManagement.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    }

    public class AuthService : IAuthService
    {
        private readonly ExternalPosApiClient _externalApiClient;
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ExternalPosApiClient externalApiClient,
            AppDbContext dbContext,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _externalApiClient = externalApiClient;
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var posResponse = await _externalApiClient.GetLoginDataAsync(
                request.Username, request.Password, cancellationToken);

            if (posResponse is null
                || posResponse.StatusCode != 200
                || posResponse.ResponseBody is not { Count: > 0 } users)
            {
                _logger.LogWarning("Login failed for {Username}", request.Username);
                throw new ApiException(StatusCodes.Status401Unauthorized, "Invalid email or password.");
            }

            var user = users[0];

            await UpsertLocationsAsync(user, cancellationToken);

            return new LoginResponseDto
            {
                Token = GenerateJwtToken(user),
                UserCode = user.UserCode ?? string.Empty,
                UserDisplayName = user.UserDisplayName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Locations = (user.UserLocations ?? new List<PosLocation>())
                    .Where(l => !string.IsNullOrWhiteSpace(l.LocationCode))
                    .Select(l => new LocationDto
                    {
                        LocationCode = l.LocationCode!,
                        LocationName = l.LocationName ?? string.Empty
                    })
                    .OrderBy(l => l.LocationName)
                    .ToList()
            };
        }

        private async Task UpsertLocationsAsync(PosLoginUser user, CancellationToken cancellationToken)
        {
            var locations = user.UserLocations ?? new List<PosLocation>();

            foreach (var location in locations)
            {
                if (string.IsNullOrWhiteSpace(location.LocationCode))
                    continue;

                var existing = await _dbContext.LocationDetails.FindAsync(location.LocationCode);

                if (existing is null)
                {
                    _dbContext.LocationDetails.Add(new LocationDetail
                    {
                        LocationCode = location.LocationCode,
                        LocationName = location.LocationName ?? string.Empty
                    });
                }
                else if (!string.IsNullOrWhiteSpace(location.LocationName)
                         && existing.LocationName != location.LocationName)
                {
                    existing.LocationName = location.LocationName;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private string GenerateJwtToken(PosLoginUser user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var parsed) ? parsed : 60;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserCode ?? string.Empty),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.UniqueName, user.UserDisplayName ?? string.Empty),
                new("company_code", user.CompanyCode ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
