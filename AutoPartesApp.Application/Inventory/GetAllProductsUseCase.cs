using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Core.Application.DTOs.Common;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class GetAllProductsUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResultDto<ProductListItemDto>> Execute(ProductFilterDto filter)
        {
            var (products, totalCount) = await _productRepository.GetPagedAsync(
                filter.SearchQuery,
                filter.CategoryId,
                filter.StockStatus,
                filter.MinStock,
                filter.MaxStock,
                filter.IsActive,
                filter.PageNumber,
                filter.PageSize
            );

            var items = products.Select(p => MapToListItemDto(p)).ToList();

            return new PagedResultDto<ProductListItemDto>(
                items,
                totalCount,
                filter.PageNumber,
                filter.PageSize
            );
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
