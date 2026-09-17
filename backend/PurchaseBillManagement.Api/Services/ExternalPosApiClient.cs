using System.Net.Http.Json;
using System.Text.Json;
using PurchaseBillManagement.Api.DTOs.External;
using PurchaseBillManagement.Api.Services.Exceptions;

namespace PurchaseBillManagement.Api.Services
{
    public class ExternalPosApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ExternalPosApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PosApiResponse<List<PosLoginUser>>?> GetLoginDataAsync(
            string username, string password, CancellationToken cancellationToken = default)
        {

            var requestBody = new PosLoginRequestDto
            {
                ApiAction = _configuration["ExternalApi:LoginMethod"] ?? "GetLoginData",
                DeviceId = _configuration["ExternalApi:DeviceId"] ?? "D001",
                SyncTime = string.Empty,
                CompanyCode = username,
                ApiBody = new PosLoginRequestBody
                {
                    Username = username,
                    Password = password
                }
            };

            using var requestContent = JsonContent.Create(requestBody, options: JsonOptions);
            using var response = await _httpClient.PostAsync(
                "api/External_Api/POS_Api/Invoke", requestContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(StatusCodes.Status502BadGateway,
                    "The external POS authentication service is unavailable. Please try again later.");
            }

            return await response.Content.ReadFromJsonAsync<PosApiResponse<List<PosLoginUser>>>(JsonOptions, cancellationToken);
        }
    }
}
