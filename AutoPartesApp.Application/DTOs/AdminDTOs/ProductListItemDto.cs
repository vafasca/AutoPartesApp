using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class ProductListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;

        // Precio
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";

        // Stock
        public int Stock { get; set; }
        public string StockStatus { get; set; } = string.Empty;

        // Categoría
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        // Imagen principal
        public string? ImageUrl { get; set; }

        // Estado
        public bool IsActive { get; set; }

        // Propiedades calculadas
        public string FormattedPrice => $"{Price:C} {Currency}";
        public bool IsLowStock => Stock > 0 && Stock <= 10;
        public bool IsOutOfStock => Stock == 0;
    }
}
