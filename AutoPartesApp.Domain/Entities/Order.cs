using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Entities
{
    public class Order
    {
        public string Id { get; set; }

        // Nuevo: número de orden legible
        public string OrderNumber { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public string? DeliveryId { get; set; }
        public OrderStatus Status { get; set; }

        // Ajuste: usar Money como Total (para EF OwnsOne)
        public Money Total { get; set; } = new Money(0, "USD");

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Ajuste: usar Value Object Address en lugar de campos planos
        public Address DeliveryAddress { get; set; } = null!;

        // Metadata
        public string? Notes { get; set; }

        // Navegación
        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public virtual Delivery? Delivery { get; set; }

    }
}
