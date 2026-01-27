using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPartesApp.Infrastructure.Persistence.Seed
{
    public static class DeliverySeed
    {
        public static List<Delivery> GetDeliveries(List<Order> orders, List<User> users)
        {
            // Tomamos un usuario con rol Delivery
            var deliveryUser = users.First(u => u.RoleType == RoleType.Delivery);

            // Tomamos una orden de prueba
            var order = orders.First();

            return new List<Delivery>
            {
                new Delivery
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = order.Id,
                    DriverId = deliveryUser.Id,
                    Status = DeliveryStatus.Delivered,
                    AssignedAt = DateTime.UtcNow.AddDays(-4),
                    DeliveredAt = DateTime.UtcNow.AddDays(-3),
                    VehicleType = "Moto",
                    VehiclePlate = "1234-ABC"
                }
            };
        }
    }
}