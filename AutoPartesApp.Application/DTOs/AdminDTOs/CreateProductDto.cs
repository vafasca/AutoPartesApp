using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(300, ErrorMessage = "El nombre no puede exceder 300 caracteres")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "El SKU es requerido")]
        [StringLength(100, ErrorMessage = "El SKU no puede exceder 100 caracteres")]
        public string Sku { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        public string CategoryId { get; set; } = string.Empty;

        // Compatibilidad de vehículos (opcionales)
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [StringLength(50)]
        public string Year { get; set; } = string.Empty;

        [StringLength(500)]
        public string Compatibility { get; set; } = string.Empty;

        // Imagen
        [StringLength(500)]
        [Url(ErrorMessage = "La URL de la imagen no es válida")]
        public string? ImageUrl { get; set; }
    }
}
