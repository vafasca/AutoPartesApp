using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Application.DTOs
{
    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Compatibility { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ImageUrls { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Propiedad calculada para el frontend
        public string StockStatus => Stock > 10 ? "Disponible" : Stock > 0 ? "Bajo Stock" : "Sin stock";
    }
}
