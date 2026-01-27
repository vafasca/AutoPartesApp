using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoPartesApp.Domain.Entities;

namespace AutoPartesApp.Infrastructure.Persistence.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.StockQuantity)
                .IsRequired();

            builder.Property(e => e.MinimumStock)
                .IsRequired();

            builder.HasOne(d => d.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventory_Product");

            builder.HasIndex(e => e.ProductId)
                .IsUnique();
        }
    }
}