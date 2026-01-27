using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPartesApp.Infrastructure.Persistence.Seed
{
    public static class OrderSeed
    {
        public static List<Order> GetOrders(List<User> users, List<Product> products)
        {
            var client = users.First(u => u.RoleType == RoleType.Client);
            var orders = new List<Order>();

            // Generamos 3 órdenes de prueba
            for (int i = 1; i <= 3; i++)
            {
                var product = products[(i - 1) % products.Count];
                var quantity = (i % 5) + 1;

                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}-{i:D4}",
                    UserId = client.Id,
                    Status = (OrderStatus)i,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    UpdatedAt = DateTime.UtcNow.AddDays(-(i - 1)),
                    DeliveryAddress = new Address(
                        $"Av. Ejemplo {100 + i}",
                        "Cochabamba",
                        "Cercado",
                        "BO",
                        $"00{i:D2}"
                    ),
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            ProductId = product.Id,
                            Quantity = quantity,
                            UnitPrice = product.Price.Amount,
                            Subtotal = quantity * product.Price.Amount,
                            CreatedAt = DateTime.UtcNow.AddDays(-i)
                        }
                    }
                };

                // Calculamos el total
                order.Total = new Money(
                    order.Items.Sum(it => it.Subtotal),
                    "USD"
                );

                orders.Add(order);
            }

            return orders;
        }
    }
}