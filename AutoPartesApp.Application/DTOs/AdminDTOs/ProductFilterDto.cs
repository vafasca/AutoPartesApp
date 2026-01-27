using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class ProductFilterDto
    {
        // Búsqueda
        public string? SearchQuery { get; set; }

        // Filtros
        public string? CategoryId { get; set; }
        public string? StockStatus { get; set; } // "Disponible", "Bajo Stock", "Agotado", "Todos"

        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }

        public bool? IsActive { get; set; }

        // Paginación
        [Range(1, int.MaxValue, ErrorMessage = "El número de página debe ser mayor a 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
        public int PageSize { get; set; } = 20;

        // Ordenamiento (opcional para futuras mejoras)
        public string? SortBy { get; set; } // "Name", "Price", "Stock", "CreatedAt"
        public string? SortOrder { get; set; } = "asc"; // "asc" o "desc"

        // Helpers
        public int Skip => (PageNumber - 1) * PageSize;
        public int Take => PageSize;
    }
}
