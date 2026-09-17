using System.Text.Json.Serialization;

namespace PurchaseBillManagement.Api.DTOs.External
{
    public class PosApiResponse<T>
    {
        [JsonPropertyName("Status_Code")] public int StatusCode { get; set; }
        [JsonPropertyName("Sync_Time")] public string? SyncTime { get; set; }
        [JsonPropertyName("Message")] public string? Message { get; set; }
        [JsonPropertyName("Response_Body")] public T? ResponseBody { get; set; }
    }

    public class PosLoginUser
    {
        [JsonPropertyName("User_Code")] public string? UserCode { get; set; }
        [JsonPropertyName("User_Display_Name")] public string? UserDisplayName { get; set; }
        [JsonPropertyName("Email")] public string? Email { get; set; }
        [JsonPropertyName("Company_Code")] public string? CompanyCode { get; set; }
        [JsonPropertyName("User_Locations")] public List<PosLocation>? UserLocations { get; set; }
    }

    public class PosLocation
    {
        [JsonPropertyName("Location_Code")] public string? LocationCode { get; set; }
        [JsonPropertyName("Location_Name")] public string? LocationName { get; set; }
        [JsonPropertyName("Stock_Handle")] public int StockHandle { get; set; }
        [JsonPropertyName("Address")] public string? Address { get; set; }
        [JsonPropertyName("Phone")] public string? Phone { get; set; }
        [JsonPropertyName("Status")] public int Status { get; set; }
    }
}
