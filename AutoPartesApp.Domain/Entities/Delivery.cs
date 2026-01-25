using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Entities
{
    public class Delivery
    {
        public string Id { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string? DriverId { get; set; } // User con rol Repartidor
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

        // Ubicación actual
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }

        // Tiempos
        public DateTime? AssignedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? EstimatedArrival { get; set; }

        // Información del vehículo
        public string? VehicleType { get; set; }
        public string? VehiclePlate { get; set; }

        // Navegación
        public virtual Order Order { get; set; } = null!;
        public virtual User Driver { get; set; } = null!;
    }
}
