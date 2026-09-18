using Microsoft.EntityFrameworkCore;
using PurchaseBillManagement.Api.Data;
using PurchaseBillManagement.Api.DTOs.PurchaseBills;
using PurchaseBillManagement.Api.Models;
using PurchaseBillManagement.Api.Services.Exceptions;

namespace PurchaseBillManagement.Api.Services
{
    public interface IPurchaseBillService
    {
        Task<PurchaseBillResponseDto> CreateAsync(
            PurchaseBillRequestDto request, string userCode, string userDisplayName, string companyCode,
            CancellationToken cancellationToken = default);

        Task<List<PurchaseBillResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }

    public class PurchaseBillService : IPurchaseBillService
    {
        private readonly AppDbContext _dbContext;

        public PurchaseBillService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PurchaseBillResponseDto> CreateAsync(
            PurchaseBillRequestDto request, string userCode, string userDisplayName, string companyCode,
            CancellationToken cancellationToken = default)
        {
            if (request.Items is null || request.Items.Count == 0)
                throw new ApiException(StatusCodes.Status400BadRequest, "A purchase bill must contain at least one item.");

            var items = new List<PurchaseBillItem>(request.Items.Count);
            foreach (var item in request.Items)
            {
                var batchLocationName = item.BatchLocationName.Trim();
                var batchExists = await _dbContext.LocationDetails
                    .AnyAsync(l => l.LocationName == batchLocationName, cancellationToken);
                if (!batchExists)
                    throw new ApiException(StatusCodes.Status400BadRequest, $"The batch location '{batchLocationName}' is invalid.");

                var grossCost = item.StandardCost * item.Quantity;
                var discountAmount = grossCost * item.DiscountPercent / 100m;
                var totalCost = grossCost - discountAmount;
                var totalSelling = item.StandardPrice * item.Quantity;

                items.Add(new PurchaseBillItem
                {
                    ItemName = item.ItemName.Trim(),
                    BatchLocationName = batchLocationName,
                    StandardCost = item.StandardCost,
                    StandardPrice = item.StandardPrice,
                    Quantity = item.Quantity,
                    DiscountPercent = item.DiscountPercent,
                    TotalCost = totalCost,
                    TotalSelling = totalSelling
                });
            }

            var bill = new PurchaseBill
            {
                BillNumber = GenerateBillNumber(),
                CompanyCode = companyCode,
                UserCode = userCode,
                UserDisplayName = userDisplayName,
                TotalItems = items.Count,
                TotalQuantity = items.Sum(i => i.Quantity),
                TotalCost = items.Sum(i => i.TotalCost),
                TotalSelling = items.Sum(i => i.TotalSelling),
                Items = items
            };

            _dbContext.PurchaseBills.Add(bill);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapToResponse(bill);
        }

        public async Task<List<PurchaseBillResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var bills = await _dbContext.PurchaseBills
                .Include(b => b.Items)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);

            return bills.Select(MapToResponse).ToList();
        }

        private static string GenerateBillNumber()
            => $"PB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        private static PurchaseBillResponseDto MapToResponse(PurchaseBill bill) => new()
        {
            Id = bill.Id,
            BillNumber = bill.BillNumber,
            TotalItems = bill.TotalItems,
            TotalQuantity = bill.TotalQuantity,
            TotalCost = bill.TotalCost,
            TotalSelling = bill.TotalSelling,
            CreatedAt = bill.CreatedAt,
            Items = bill.Items.Select(i => new PurchaseBillItemResponseDto
            {
                Id = i.Id,
                ItemName = i.ItemName,
                BatchLocationName = i.BatchLocationName,
                StandardCost = i.StandardCost,
                StandardPrice = i.StandardPrice,
                Quantity = i.Quantity,
                DiscountPercent = i.DiscountPercent,
                FreeQuantity = 0,
                Margin = i.StandardPrice <= 0 ? 0 : (i.StandardPrice - i.StandardCost) / i.StandardPrice * 100,
                TotalCost = i.TotalCost,
                TotalSelling = i.TotalSelling
            }).ToList()
        };
    }
}
