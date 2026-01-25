using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Entities
{
    public class OrderItem
    {
        public string Id { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navegación
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
