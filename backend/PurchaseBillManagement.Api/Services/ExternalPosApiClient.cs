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
            string companyCode, string username, string password, CancellationToken cancellationToken = default)
        {
            // Dev/test mode: stub the external API so the whole flow can be tested offline.
            if (_configuration.GetValue<bool>("ExternalApi:UseMock"))
            {
                return BuildMockLoginResponse();
            }

            var requestBody = new PosLoginRequestDto
            {
                ApiAction = _configuration["ExternalApi:LoginMethod"] ?? "GetLoginData",
                DeviceId = _configuration["ExternalApi:DeviceId"] ?? "D001",
                SyncTime = string.Empty,
                CompanyCode = companyCode,
                ApiBody = new PosLoginRequestBody
                {
                    Username = username,
                    Pw = password
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

        // Mirror of the real "GetLoginData" response (trimmed to a few locations for offline testing).
        private static PosApiResponse<List<PosLoginUser>> BuildMockLoginResponse() => new()
        {
            StatusCode = 200,
            SyncTime = "",
            Message = "GetLoginData POS API Executed Successfully. (MOCK)",
            ResponseBody = new List<PosLoginUser>
            {
                new()
                {
                    UserCode = "EZCMP1/EZUSR-1",
                    UserDisplayName = "eZuite Admin",
                    Email = "info@enhanzer.com",
                    CompanyCode = "EZCMP-1",
                    UserLocations = new List<PosLocation>
                    {
                        new() { LocationCode = "EZCMP1/EZLOC-16", LocationName = "Head Office", StockHandle = 1 },
                        new() { LocationCode = "EZCMP1/EZLOC-29", LocationName = "Block C", StockHandle = 1 },
                        new() { LocationCode = "EZCMP1/EZLOC-23", LocationName = "Main Warehouse", StockHandle = 1 },
                        new() { LocationCode = "EZCMP1/EZLOC-17", LocationName = "Stores", StockHandle = 1 },
                        new() { LocationCode = "EZCMP1/EZLOC-18", LocationName = "Warehouse 1", StockHandle = 1 },
                        new() { LocationCode = "EZCMP1/EZLOC-2", LocationName = "Demo Company", StockHandle = 1 }
                    }
                }
            }
        };
    }
}
