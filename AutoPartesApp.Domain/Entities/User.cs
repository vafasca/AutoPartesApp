using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public RoleType RoleType { get; set; }

        // Campos adicionales
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        // Address como objeto complejo
        public string? AddressStreet { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressState { get; set; }
        public string? AddressZipCode { get; set; }

        // Navegación
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // Propiedades calculadas para compatibilidad con tu código actual
        public string Role => RoleType.ToFriendlyString();

    }
}
