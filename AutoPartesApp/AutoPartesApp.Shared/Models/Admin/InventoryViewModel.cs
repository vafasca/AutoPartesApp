using System;
using System.Collections.Generic;
using AutoPartesApp.Application.DTOs.InventoryDTOs;

namespace AutoPartesApp.Shared.Models.Admin
{
    public class InventoryViewModel
    {
        public IEnumerable<InventoryItemDto> Items { get; set; }
        public int TotalItems { get; set; }
        public int TotalStock { get; set; }
        public int LowStockItemsCount { get; set; }
        public Dictionary<string, int> StockByCategory { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class InventoryStat
    {
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string TrendText { get; set; } = string.Empty;
        public string TrendColor { get; set; } = string.Empty;
    }

    public class FilterOption
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ProductItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int MinimumStock { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsLowStock => Stock <= MinimumStock;
    }
}
