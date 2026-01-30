using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetInventoryReportUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetInventoryReportUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<InventoryReportDto> ExecuteAsync()
        {
            var totalProducts = await _productRepository.GetTotalCountAsync();
            var totalValue = await _productRepository.GetTotalValueAsync();
            var outOfStock = await _productRepository.GetOutOfStockCountAsync();
            var lowStock = await _productRepository.GetLowStockCountAsync(10);

            // Obtener todos los productos para calcular valor por categoría
            var allProducts = await _productRepository.GetAllAsync();

            var valueByCategory = allProducts
                .GroupBy(p => new
                {
                    p.Category.Id,
                    p.Category.Name
                })
                .Select(g => new
                {
                    CategoryName = g.Key.Name,
                    TotalValue = g.Sum(p => p.Price.Amount * p.Stock),
                    TotalUnits = g.Sum(p => p.Stock)
                })
                .OrderByDescending(c => c.TotalValue)
                .ToList();

            var categoryValueList = valueByCategory.Select(c => new CategoryValueDto
            {
                CategoryName = c.CategoryName,
                TotalValue = c.TotalValue,
                TotalUnits = c.TotalUnits,
                Percentage = totalValue > 0 ? (c.TotalValue / totalValue) * 100 : 0
            }).ToList();

            var availableProducts = totalProducts - outOfStock;
            var averageProductValue = totalProducts > 0 ? totalValue / totalProducts : 0;

            return new InventoryReportDto
            {
                TotalProducts = totalProducts,
                TotalValue = totalValue,
                OutOfStock = outOfStock,
                LowStock = lowStock,
                ValueByCategory = categoryValueList,
                AverageProductValue = averageProductValue,
                AvailableProducts = availableProducts
            };
        }
    }
}
