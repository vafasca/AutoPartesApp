namespace AutoPartesApp.Application.DTOs.InventoryDTOs
{
    public class UpdateStockDto
    {
        public int ProductId { get; set; }
        public int QuantityChange { get; set; } // Positive to add, negative to subtract
        public string Reason { get; set; }
    }
}