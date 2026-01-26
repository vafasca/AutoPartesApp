using AutoPartesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesApp.Infrastructure.Persistence
{
    public class AutoPartesDbContext : DbContext
    {
        public AutoPartesDbContext(DbContextOptions<AutoPartesDbContext> options)
            : base(options)
        {
        }

        // DbSets - Tablas de la base de datos
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Delivery> Deliveries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones desde archivos separados
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AutoPartesDbContext).Assembly);

            // Ignorar propiedad calculada en User
            modelBuilder.Entity<User>().Ignore(u => u.Role);

            // Relaciones explícitas para claves tipo string
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Delivery)
                .WithOne(d => d.Order)
                .HasForeignKey<Delivery>(d => d.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Driver)
                .WithMany() // si no tienes una colección en User
                .HasForeignKey(d => d.DriverId);

            // Configurar precisión de decimales en OrderItem
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Subtotal)
                .HasPrecision(18, 2);

            // Seed data inicial
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = DateTime.SpecifyKind(new DateTime(2024, 01, 01), DateTimeKind.Utc);

            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = "1", Name = "Motor", Description = "Repuestos de motor", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = "2", Name = "Frenos", Description = "Sistema de frenado", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = "3", Name = "Suspensión", Description = "Amortiguadores y suspensión", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = "4", Name = "Eléctrico", Description = "Sistema eléctrico", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = "5", Name = "Filtros", Description = "Filtros de aire, aceite, combustible", CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = "6", Name = "Llantas", Description = "Neumáticos y llantas", CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Usuarios de prueba
            modelBuilder.Entity<User>().HasData(
                // Admin
                new User
                {
                    Id = "admin-001",
                    Email = "admin@autopartes.com",
                    FullName = "Administrador Principal",
                    Phone = "+52 555-0001",
                    PasswordHash = "admin123", // ⚠️ TEMPORAL - en producción usar BCrypt
                    RoleType = Domain.Enums.RoleType.Admin,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                // Delivery
                new User
                {
                    Id = "delivery-001",
                    Email = "delivery@autopartes.com",
                    FullName = "Roberto Sánchez",
                    Phone = "+52 555-0002",
                    PasswordHash = "delivery123",
                    RoleType = Domain.Enums.RoleType.Delivery,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                },
                // Client
                new User
                {
                    Id = "client-001",
                    Email = "client@autopartes.com",
                    FullName = "Carlos Rodríguez",
                    Phone = "+52 555-0003",
                    PasswordHash = "client123",
                    RoleType = Domain.Enums.RoleType.Client,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );
        }


    }
}
