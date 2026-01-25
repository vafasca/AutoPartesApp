using AutoPartesApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Entities
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public Money Price { get; set; } = new Money(0, "USD");
        public int Stock { get; set; }
        public string CategoryId { get; set; } = string.Empty;

        // Compatibilidad de vehículos
        public string Compatibility { get; set; } = string.Empty; // "Toyota-Corolla-2018,Honda-Civic-2020"
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;

        // Imágenes
        public string? ImageUrl { get; set; }
        public string? ImageUrls { get; set; } // JSON array de URLs adicionales

        // Metadata
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navegación
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Propiedades calculadas
        public string StockStatus => Stock > 10 ? "Disponible" : Stock > 0 ? "Bajo Stock" : "Agotado";
    }
}
