using System;
using System.Collections.Generic;
using System.Text;
using AutoPartesApp.Application.DTOs;

namespace AutoPartesApp.Shared.Models.Admin
{
    public class InventoryViewModel
    {
        public List<ProductDto> Products { get; set; } = new();
        public InventoryStats Stats { get; set; } = new();
        public int TotalProducts { get; set; }
    }

    public class InventoryStats
    {
        public int TotalProducts { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }

    public class ProductItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
