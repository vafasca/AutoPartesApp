using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class GetProductByIdUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> Execute(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                return null;

            return MapToDto(product);
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
