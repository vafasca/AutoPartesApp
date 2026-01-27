using AutoPartesApp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace AutoPartesApp.Infrastructure.Persistence.Seed
{
    public static class CategorySeed
    {
        public static List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category { Id = Guid.NewGuid().ToString(), Name = "Frenos" },
                new Category { Id = Guid.NewGuid().ToString(), Name = "Motor" },
                new Category { Id = Guid.NewGuid().ToString(), Name = "Suspensión" },
                new Category { Id = Guid.NewGuid().ToString(), Name = "Eléctrico" }
            };
        }
    }
}