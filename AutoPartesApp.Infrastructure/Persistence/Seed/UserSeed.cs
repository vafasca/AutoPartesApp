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
            var seedDate = DateTime.UtcNow;

            return new List<User>
            {
                // ========================================
                // ADMINISTRADORES
                // ========================================
                new User
                {
                    Id = "admin-001",
                    FullName = "Administrador Principal",
                    Email = "admin@autopartes.com",
                    PasswordHash = "admin123", // ⚠️ Solo para pruebas
                    RoleType = RoleType.Admin,
                    Phone = "+591 70000001",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Admin&background=6366f1&color=fff",
                    IsActive = true,
                    AddressStreet = "Av. América #500",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00001",
                    CreatedAt = seedDate.AddMonths(-12),
                    UpdatedAt = seedDate,
                    LastLoginAt = seedDate.AddHours(-2)
                },

                // ========================================
                // CLIENTES
                // ========================================
                new User
                {
                    Id = "client-001",
                    FullName = "Carlos Mendoza",
                    Email = "carlos.mendoza@email.com",
                    PasswordHash = "client123",
                    RoleType = RoleType.Client,
                    Phone = "+591 70000002",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Carlos+Mendoza&background=10b981&color=fff",
                    IsActive = true,
                    AddressStreet = "Av. Heroínas #234",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00002",
                    CreatedAt = seedDate.AddMonths(-8),
                    UpdatedAt = seedDate.AddDays(-5),
                    LastLoginAt = seedDate.AddHours(-6)
                },
                new User
                {
                    Id = "client-002",
                    FullName = "Elena Rodríguez",
                    Email = "elena.rodriguez@email.com",
                    PasswordHash = "client123",
                    RoleType = RoleType.Client,
                    Phone = "+591 70000003",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Elena+Rodriguez&background=ec4899&color=fff",
                    IsActive = true,
                    AddressStreet = "Calle Jordán #456",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00003",
                    CreatedAt = seedDate.AddMonths(-6),
                    UpdatedAt = seedDate.AddDays(-10),
                    LastLoginAt = seedDate.AddDays(-4)
                },
                new User
                {
                    Id = "client-003",
                    FullName = "Miguel Ángel Torres",
                    Email = "miguel.torres@email.com",
                    PasswordHash = "client123",
                    RoleType = RoleType.Client,
                    Phone = "+591 70000004",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Miguel+Torres&background=f59e0b&color=fff",
                    IsActive = true,
                    AddressStreet = "Av. Blanco Galindo Km 4",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00004",
                    CreatedAt = seedDate.AddMonths(-10),
                    UpdatedAt = seedDate.AddDays(-1),
                    LastLoginAt = seedDate.AddHours(-12)
                },
                new User
                {
                    Id = "client-004",
                    FullName = "Ana Martínez",
                    Email = "ana.martinez@email.com",
                    PasswordHash = "client123",
                    RoleType = RoleType.Client,
                    Phone = "+591 70000005",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Ana+Martinez&background=8b5cf6&color=fff",
                    IsActive = true,
                    AddressStreet = "Calle España #789",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00005",
                    CreatedAt = seedDate.AddMonths(-4),
                    UpdatedAt = seedDate.AddDays(-15),
                    LastLoginAt = seedDate.AddDays(-14)
                },
                new User
                {
                    Id = "client-005",
                    FullName = "Roberto Fernández",
                    Email = "roberto.fernandez@email.com",
                    PasswordHash = "client123",
                    RoleType = RoleType.Client,
                    Phone = "+591 70000006",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Roberto+Fernandez&background=14b8a6&color=fff",
                    IsActive = false, // Usuario inactivo
                    AddressStreet = "Av. Ramón Rivero #321",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00006",
                    CreatedAt = seedDate.AddMonths(-3),
                    UpdatedAt = seedDate.AddMonths(-1),
                    LastLoginAt = seedDate.AddMonths(-1)
                },

                // ========================================
                // REPARTIDORES
                // ========================================
                new User
                {
                    Id = "delivery-001",
                    FullName = "Roberto Sánchez",
                    Email = "roberto.sanchez@delivery.com",
                    PasswordHash = "delivery123",
                    RoleType = RoleType.Delivery,
                    Phone = "+591 70000007",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Roberto+Sanchez&background=3b82f6&color=fff",
                    IsActive = true,
                    AddressStreet = "Zona Sud #111",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00007",
                    CreatedAt = seedDate.AddMonths(-9),
                    UpdatedAt = seedDate.AddMinutes(-30),
                    LastLoginAt = seedDate.AddMinutes(-30)
                },
                new User
                {
                    Id = "delivery-002",
                    FullName = "Lucía Gómez",
                    Email = "lucia.gomez@delivery.com",
                    PasswordHash = "delivery123",
                    RoleType = RoleType.Delivery,
                    Phone = "+591 70000008",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Lucia+Gomez&background=ec4899&color=fff",
                    IsActive = true,
                    AddressStreet = "Barrio Temporal #222",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00008",
                    CreatedAt = seedDate.AddMonths(-7),
                    UpdatedAt = seedDate.AddHours(-3),
                    LastLoginAt = seedDate.AddHours(-3)
                },
                new User
                {
                    Id = "delivery-003",
                    FullName = "Pedro Ramírez",
                    Email = "pedro.ramirez@delivery.com",
                    PasswordHash = "delivery123",
                    RoleType = RoleType.Delivery,
                    Phone = "+591 70000009",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Pedro+Ramirez&background=10b981&color=fff",
                    IsActive = true,
                    AddressStreet = "Av. Circunvalación #333",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00009",
                    CreatedAt = seedDate.AddMonths(-5),
                    UpdatedAt = seedDate.AddMinutes(-45),
                    LastLoginAt = seedDate.AddMinutes(-45)
                },
                new User
                {
                    Id = "delivery-004",
                    FullName = "Carmen Silva",
                    Email = "carmen.silva@delivery.com",
                    PasswordHash = "delivery123",
                    RoleType = RoleType.Delivery,
                    Phone = "+591 70000010",
                    AvatarUrl = "https://ui-avatars.com/api/?name=Carmen+Silva&background=f59e0b&color=fff",
                    IsActive = true,
                    AddressStreet = "Villa San Antonio #444",
                    AddressCity = "Cochabamba",
                    AddressState = "Cochabamba",
                    AddressCountry = "Bolivia",
                    AddressZipCode = "00010",
                    CreatedAt = seedDate.AddMonths(-6),
                    UpdatedAt = seedDate.AddHours(-1),
                    LastLoginAt = seedDate.AddHours(-1)
                }
            };
        }
    }
}