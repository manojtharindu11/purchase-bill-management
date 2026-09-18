namespace PurchaseBillManagement.Api.Models
{
    public class PurchaseBill
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalSelling { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<PurchaseBillItem> Items { get; set; } = new();
    }
}
