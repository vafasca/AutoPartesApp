using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class SearchProductsUseCase
    {
        private readonly IProductRepository _productRepository;

        public SearchProductsUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductListItemDto>> Execute(string query)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(query))
                return new List<ProductListItemDto>();

            var products = await _productRepository.SearchAsync(query);

            return products.Select(p => MapToListItemDto(p)).ToList();
        }

        private ProductListItemDto MapToListItemDto(Product product)
        {
            return new ProductListItemDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price.Amount,
                Currency = product.Price.Currency,
                Stock = product.Stock,
                StockStatus = product.StockStatus,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "",
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive
            };
        }
    }
}
