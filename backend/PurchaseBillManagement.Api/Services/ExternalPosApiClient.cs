using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using PurchaseBillManagement.Api.DTOs.External;
using PurchaseBillManagement.Api.Services.Exceptions;

namespace PurchaseBillManagement.Api.Services
{
    public class ExternalPosApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalPosApiClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ExternalPosApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<ExternalPosApiClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
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

                    // Send JSON explicitly using StringContent (avoid JsonContent.Create quirks).
                    var serialized = JsonSerializer.Serialize(requestBody, JsonOptions);
                    using var requestContent = new StringContent(serialized, Encoding.UTF8, "application/json");
                    using var response = await _httpClient.PostAsync("api/External_Api/POS_Api/Invoke", requestContent, cancellationToken);

                    string content = string.Empty;
                    try
                    {
                        content = await response.Content.ReadAsStringAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to read external POS response body.");
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("External POS returned non-success status code {StatusCode}.", response.StatusCode);
                        throw new ApiException(StatusCodes.Status502BadGateway, "The external POS authentication service is unavailable. Please try again later.");
                    }

                    PosApiResponse<List<PosLoginUser>>? result = null;
                    try
                    {
                        if (!string.IsNullOrEmpty(content))
                        {
                            result = JsonSerializer.Deserialize<PosApiResponse<List<PosLoginUser>>>(content, JsonOptions);
                        }
                        else
                        {
                            result = await response.Content.ReadFromJsonAsync<PosApiResponse<List<PosLoginUser>>>(JsonOptions, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize external POS response.");
                        throw new ApiException(StatusCodes.Status502BadGateway, "Invalid response from external POS service.");
                    }

                    if (result is null || result.StatusCode == 0 || result.ResponseBody is null || result.ResponseBody.Count == 0)
                    {
                        _logger.LogWarning("External POS returned invalid payload. StatusCode={StatusCode}, BodyPresent={BodyPresent}.", result?.StatusCode ?? 0, result?.ResponseBody is not null);
                        throw new ApiException(StatusCodes.Status502BadGateway, "Invalid response from external POS service.");
                    }

                    return result;
                }
    }
}
