using AutoPartesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.Property(o => o.UserId)
                .IsRequired();

            // Value Object - Money (Total)
            builder.OwnsOne(o => o.Total, total =>
            {
                total.Property(m => m.Amount)
                    .HasColumnName("TotalAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                total.Property(m => m.Currency)
                    .HasColumnName("TotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            // Value Object - Address
            builder.OwnsOne(o => o.DeliveryAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("DeliveryStreet")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("DeliveryCity")
                    .HasMaxLength(100);

                address.Property(a => a.State)
                    .HasColumnName("DeliveryState")
                    .HasMaxLength(100);

                address.Property(a => a.Country)
                    .HasColumnName("DeliveryCountry")
                    .HasMaxLength(100);

                address.Property(a => a.ZipCode)
                    .HasColumnName("DeliveryZipCode")
                    .HasMaxLength(20);
            });

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.UpdatedAt);

            // Relaciones
            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Delivery>()
                .WithOne()
                .HasForeignKey<Delivery>("OrderId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
