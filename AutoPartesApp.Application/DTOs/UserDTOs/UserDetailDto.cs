using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.DTOs.UserDTOs
{
    public class UserDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Rol
        public string Role { get; set; } = string.Empty;
        public RoleType RoleType { get; set; }

        // Avatar
        public string? AvatarUrl { get; set; }

        // Estado
        public bool IsActive { get; set; }

        // Dirección completa
        public string? AddressStreet { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressState { get; set; }
        public string? AddressCountry { get; set; }
        public string? AddressZipCode { get; set; }

        // Dirección formateada
        public string FullAddress => BuildFullAddress();

        // Fechas
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Estadísticas según el rol
        public UserOrderSummaryDto? OrdersSummary { get; set; } // Para clientes
        public UserDeliverySummaryDto? DeliveriesSummary { get; set; } // Para repartidores

        private string BuildFullAddress()
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(AddressStreet)) parts.Add(AddressStreet);
            if (!string.IsNullOrEmpty(AddressCity)) parts.Add(AddressCity);
            if (!string.IsNullOrEmpty(AddressState)) parts.Add(AddressState);
            if (!string.IsNullOrEmpty(AddressZipCode)) parts.Add(AddressZipCode);
            if (!string.IsNullOrEmpty(AddressCountry)) parts.Add(AddressCountry);

            return parts.Count > 0 ? string.Join(", ", parts) : "Sin dirección registrada";
        }
    }
}
