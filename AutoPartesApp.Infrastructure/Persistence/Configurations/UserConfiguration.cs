using AutoPartesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            // Email único
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            // FullName
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);

            // Phone
            builder.Property(u => u.Phone)
                .HasMaxLength(20);

            // RoleType como entero
            builder.Property(u => u.RoleType)
                .IsRequired()
                .HasConversion<int>();

            // IsActive
            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Fechas
            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired();

            builder.Property(u => u.LastLoginAt);

            // 🆕 Configuración de Address (campos separados)
            builder.Property(u => u.AddressStreet)
                .HasMaxLength(200);

            builder.Property(u => u.AddressCity)
                .HasMaxLength(100);

            builder.Property(u => u.AddressState)
                .HasMaxLength(100);

            builder.Property(u => u.AddressCountry)
                .HasMaxLength(100);

            builder.Property(u => u.AddressZipCode)
                .HasMaxLength(20);

            // Avatar
            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            // Relaciones
            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
