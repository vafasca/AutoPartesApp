using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,      // Pendiente
        Confirmed = 2,    // Confirmado
        Processing = 3,   // En preparación
        Shipped = 4,      // Enviado
        OnRoute = 5,      // En camino (en manos del repartidor)
        Delivered = 6,    // Entregado
        Cancelled = 7     // Cancelado
    }
}
