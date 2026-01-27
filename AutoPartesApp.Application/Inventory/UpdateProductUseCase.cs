using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class UpdateProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> Execute(string productId, UpdateProductDto dto)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre del producto es requerido");

            if (dto.Price <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0");

            // Buscar producto existente
            var existingProduct = await _productRepository.GetByIdAsync(productId);
            if (existingProduct == null)
                return null;

            // Actualizar propiedades
            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.Price = new Money(dto.Price, dto.Currency);
            existingProduct.Stock = dto.Stock;
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.Brand = dto.Brand;
            existingProduct.Model = dto.Model;
            existingProduct.Year = dto.Year;
            existingProduct.Compatibility = dto.Compatibility;
            existingProduct.ImageUrl = dto.ImageUrl;
            existingProduct.IsActive = dto.IsActive;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            // Guardar cambios
            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);

            // Recargar con la categoría
            updatedProduct = await _productRepository.GetByIdAsync(updatedProduct.Id);

            // Mapear entidad a DTO
            return MapToDto(updatedProduct!);
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
