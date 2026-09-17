using System.ComponentModel.DataAnnotations;

namespace PurchaseBillManagement.Api.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Company code is required.")]
        public string CompanyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Pw { get; set; } = string.Empty;
    }
}
