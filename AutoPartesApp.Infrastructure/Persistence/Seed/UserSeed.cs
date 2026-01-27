using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AutoPartesApp.Infrastructure.Persistence.Seed
{
    public static class UserSeed
    {
        public static List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Administrador General",
                    Email = "admin@autopartes.com",
                    PasswordHash = "admin123", // ⚠️ Solo para pruebas
                    RoleType = RoleType.Admin,
                    Phone = "70000001",
                    AvatarUrl = "https://i.pravatar.cc/150?img=1",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6),
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Juan Pérez",
                    Email = "juan@cliente.com",
                    PasswordHash = "123456",
                    RoleType = RoleType.Client,
                    Phone = "70000002",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Carlos Delivery",
                    Email = "delivery@autopartes.com",
                    PasswordHash = "123456",
                    RoleType = RoleType.Delivery,
                    Phone = "70000003",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }
    }
}