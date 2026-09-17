using System.ComponentModel.DataAnnotations;

namespace PurchaseBillManagement.Api.DTOs.PurchaseBills
{
    public class PurchaseBillRequestDto
    {
        [Required(ErrorMessage = "Batch location is required.")]
        public string BatchLocationName { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "Add at least one item to the bill.")]
        public List<PurchaseBillItemRequestDto> Items { get; set; } = new();
    }

    public class PurchaseBillItemRequestDto
    {
        [Required(ErrorMessage = "Item name is required.")]
        public string ItemName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "99999999999", ErrorMessage = "Standard cost must be zero or more.")]
        public decimal StandardCost { get; set; }

        [Range(typeof(decimal), "0", "99999999999", ErrorMessage = "Standard price must be zero or more.")]
        public decimal StandardPrice { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0", "100", ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal DiscountPercent { get; set; }
    }

    public class PurchaseBillResponseDto
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public string BatchLocationName { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalSelling { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseBillItemResponseDto> Items { get; set; } = new();
    }

    public class PurchaseBillItemResponseDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal StandardCost { get; set; }
        public decimal StandardPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalSelling { get; set; }
    }
}
