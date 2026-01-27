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

            // Generamos 100 órdenes de prueba
            for (int i = 1; i <= 100; i++)
            {
                var product = products[i % products.Count];

                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderNumber = $"ORD-{i:D4}",
                    UserId = client.Id,
                    Status = (OrderStatus)(i % Enum.GetValues(typeof(OrderStatus)).Length),
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    UpdatedAt = DateTime.UtcNow.AddDays(-(i - 1)),
                    DeliveryAddress = new Address(
                        "Av. Siempre Viva 123",
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
                            Quantity = (i % 5) + 1, // cantidades entre 1 y 5
                            UnitPrice = product.Price.Amount,
                            Subtotal = ((i % 5) + 1) * product.Price.Amount,
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