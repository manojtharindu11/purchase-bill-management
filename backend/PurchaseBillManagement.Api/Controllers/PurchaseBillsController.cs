using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBillManagement.Api.DTOs.PurchaseBills;
using PurchaseBillManagement.Api.Services;

namespace PurchaseBillManagement.Api.Controllers
{
    [ApiController]
    [Route("api/purchasebills")]
    [Authorize]
    public class PurchaseBillsController : ControllerBase
    {
        private readonly IPurchaseBillService _purchaseBillService;

        public PurchaseBillsController(IPurchaseBillService purchaseBillService)
        {
            _purchaseBillService = purchaseBillService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseBillRequestDto request, CancellationToken cancellationToken)
        {
            var userCode = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
            var userDisplayName = User.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? string.Empty;
            var companyCode = User.FindFirstValue("company_code") ?? string.Empty;

            var bill = await _purchaseBillService.CreateAsync(
                request, userCode, userDisplayName, companyCode, cancellationToken);

            return CreatedAtAction(nameof(GetAll), new { id = bill.Id }, bill);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
            => Ok(await _purchaseBillService.GetAllAsync(cancellationToken));
    }
}
