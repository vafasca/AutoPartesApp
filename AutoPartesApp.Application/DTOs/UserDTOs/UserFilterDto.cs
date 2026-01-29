using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.UserDTOs
{
    public class UserFilterDto
    {
        // Búsqueda por texto
        public string? SearchQuery { get; set; }

        // Filtro por rol
        public RoleType? RoleType { get; set; }

        // Filtro por estado
        public bool? IsActive { get; set; }

        // Filtro por rango de fechas
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
