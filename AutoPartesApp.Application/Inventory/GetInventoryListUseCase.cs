using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPartesApp.Application.DTOs.InventoryDTOs;
using AutoPartesApp.Domain.Interfaces;

namespace AutoPartesApp.Application.Inventory
{
    public class GetInventoryListUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public GetInventoryListUseCase(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<InventoryListDto> ExecuteAsync()
        {
            var inventoryItems = await _inventoryRepository.GetAllInventoryAsync();
            
            var items = inventoryItems.Select(i => new InventoryItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "N/A",
                ProductDescription = i.Product?.Description ?? "N/A",
                Price = i.Product?.Price ?? 0,
                StockQuantity = i.StockQuantity,
                MinimumStock = i.MinimumStock,
                CategoryName = i.Product?.Category?.Name ?? "N/A",
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            }).ToList();

            var stockByCategory = items
                .GroupBy(x => x.CategoryName)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.StockQuantity));

            var dto = new InventoryListDto
            {
                Items = items,
                TotalItems = items.Count,
                TotalStock = items.Sum(x => x.StockQuantity),
                LowStockItemsCount = items.Count(x => x.IsLowStock),
                StockByCategory = stockByCategory
            };

            return dto;
        }
    }
}