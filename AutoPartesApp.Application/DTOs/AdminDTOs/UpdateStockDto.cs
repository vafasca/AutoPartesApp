using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.AdminDTOs
{
    public class UpdateStockDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nuevo stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int NewStock { get; set; }
    }
}
