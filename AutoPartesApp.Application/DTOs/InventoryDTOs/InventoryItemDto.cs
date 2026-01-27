using System;

namespace AutoPartesApp.Application.DTOs.InventoryDTOs
{
    public class InventoryItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStock { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsLowStock => StockQuantity <= MinimumStock;
    }
}