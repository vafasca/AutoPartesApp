using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Enums
{
    public enum DeliveryStatus
    {
        Pending = 1,      // Pendiente de asignar
        Assigned = 2,     // Asignado a repartidor
        PickedUp = 3,     // Recogido
        OnRoute = 4,      // En camino
        Delivered = 5,    // Entregado
        Failed = 6        // Fallido
    }
}
