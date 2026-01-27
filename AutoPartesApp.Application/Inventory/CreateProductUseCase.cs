using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class CreateProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public CreateProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> Execute(CreateProductDto dto)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre del producto es requerido");

            if (string.IsNullOrWhiteSpace(dto.Sku))
                throw new ArgumentException("El SKU es requerido");

            if (dto.Price <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0");

            // Verificar si el SKU ya existe
            var existingProduct = await _productRepository.GetBySkuAsync(dto.Sku);
            if (existingProduct != null)
                throw new InvalidOperationException($"Ya existe un producto con el SKU: {dto.Sku}");

            // Mapear DTO a entidad
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                Sku = dto.Sku,
                Price = new Money(dto.Price, dto.Currency),
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                Compatibility = dto.Compatibility,
                ImageUrl = dto.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Crear producto
            var createdProduct = await _productRepository.CreateAsync(product);

            // Recargar con la categoría
            createdProduct = await _productRepository.GetByIdAsync(createdProduct.Id);

            // Mapear entidad a DTO
            return MapToDto(createdProduct!);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Sku = product.Sku,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                Stock = product.Stock,
                StockStatus = product.StockStatus,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "",
                Brand = product.Brand,
                Model = product.Model,
                Year = product.Year,
                Compatibility = product.Compatibility,
                ImageUrl = product.ImageUrl,
                ImageUrls = product.ImageUrls,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
