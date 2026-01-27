using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPartesApp.Infrastructure.Persistence.Seed
{
    public static class ProductSeed
    {
        public static List<Product> GetProducts(List<Category> categories)
        {
            // Ejemplo: obtener categoría "Frenos"
            var frenos = categories.First(c => c.Name == "Frenos");
            var motor = categories.First(c => c.Name == "Motor");
            var suspension = categories.First(c => c.Name == "Suspensión");

            return new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Pastillas de Freno Toyota Corolla",
                    Description = "Juego de pastillas de freno delanteras",
                    Sku = "FR-TY-001",
                    Price = new Money(45.99m, "USD"),
                    Stock = 25,
                    CategoryId = frenos.Id,
                    Brand = "Bosch",
                    Model = "Corolla",
                    Year = "2018-2022",
                    Compatibility = "Toyota-Corolla-2018",
                    ImageUrl = "https://picsum.photos/400/300?random=1",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Filtro de Aceite Honda Civic",
                    Description = "Filtro de aceite original para motor Honda",
                    Sku = "MO-HC-002",
                    Price = new Money(15.50m, "USD"),
                    Stock = 40,
                    CategoryId = motor.Id,
                    Brand = "Honda",
                    Model = "Civic",
                    Year = "2016-2021",
                    Compatibility = "Honda-Civic-2016",
                    ImageUrl = "https://picsum.photos/400/300?random=2",
                    CreatedAt = DateTime.UtcNow.AddMonths(-5),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Amortiguador Trasero Ford Ranger",
                    Description = "Amortiguador trasero reforzado para camioneta Ford Ranger",
                    Sku = "SU-FR-003",
                    Price = new Money(120.00m, "USD"),
                    Stock = 10,
                    CategoryId = suspension.Id,
                    Brand = "Monroe",
                    Model = "Ranger",
                    Year = "2015-2020",
                    Compatibility = "Ford-Ranger-2015",
                    ImageUrl = "https://picsum.photos/400/300?random=3",
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }
    }
}