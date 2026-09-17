using PurchaseBillManagement.Api.DTOs.Locations;

namespace PurchaseBillManagement.Api.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;
        public List<LocationDto> Locations { get; set; } = new();
    }
}
