using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseBillManagement.Api.DTOs.PurchaseBills;
using PurchaseBillManagement.Api.Services;

namespace PurchaseBillManagement.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchasebills")]
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
            Console.WriteLine($"User Code: {userCode}"); // Log the user code for debugging

            var bill = await _purchaseBillService.CreateAsync(
                request, userCode, cancellationToken);

            return CreatedAtAction(nameof(GetAll), new { id = bill.Id }, bill);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
            => Ok(await _purchaseBillService.GetAllAsync(cancellationToken));
    }
}
