using AutoPartesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Infrastructure.Persistence.Configurations
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.ToTable("Deliveries");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.OrderId)
                .IsRequired();

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<int>();

            // Ubicación actual (opcional)
            builder.Property(d => d.CurrentLatitude)
                .HasColumnType("double precision");

            builder.Property(d => d.CurrentLongitude)
                .HasColumnType("double precision");

            // Tiempos
            builder.Property(d => d.AssignedAt);
            builder.Property(d => d.PickedUpAt);
            builder.Property(d => d.DeliveredAt);
            builder.Property(d => d.EstimatedArrival);

            // Información del vehículo
            builder.Property(d => d.VehicleType)
                .HasMaxLength(100);

            builder.Property(d => d.VehiclePlate)
                .HasMaxLength(20);

            // Relaciones
            builder.HasOne(d => d.Order)
                .WithOne(o => o.Delivery)
                .HasForeignKey<Delivery>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Driver)
                .WithMany()
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para optimizar consultas
            builder.HasIndex(d => d.OrderId)
                .IsUnique();

            builder.HasIndex(d => d.DriverId);

            builder.HasIndex(d => d.Status);

            builder.HasIndex(d => d.AssignedAt);
        }
    }
}
