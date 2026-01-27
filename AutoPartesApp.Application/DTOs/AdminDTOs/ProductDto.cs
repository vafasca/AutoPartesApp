using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;

        // Precio
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";

        // Stock
        public int Stock { get; set; }
        public string StockStatus { get; set; } = string.Empty; // "Disponible", "Bajo Stock", "Agotado"

        // Categoría
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        // Compatibilidad de vehículos
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Compatibility { get; set; } = string.Empty;

        // Imágenes
        public string? ImageUrl { get; set; }
        public string? ImageUrls { get; set; } // JSON array de URLs adicionales

        // Metadata
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Propiedades calculadas
        public decimal TotalValue => Price * Stock;
        public bool IsLowStock => Stock > 0 && Stock <= 10;
        public bool IsOutOfStock => Stock == 0;
    }
}
