using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class GetInventoryStatsUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetInventoryStatsUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<InventoryStatsDto> Execute()
        {
            // Obtener todas las estadísticas en paralelo
            var totalCountTask = _productRepository.GetTotalCountAsync();
            var totalValueTask = _productRepository.GetTotalValueAsync();
            var lowStockCountTask = _productRepository.GetLowStockCountAsync(10);
            var outOfStockCountTask = _productRepository.GetOutOfStockCountAsync();

            await Task.WhenAll(totalCountTask, totalValueTask, lowStockCountTask, outOfStockCountTask);

            var totalCount = await totalCountTask;
            var totalValue = await totalValueTask;
            var lowStockCount = await lowStockCountTask;
            var outOfStockCount = await outOfStockCountTask;

            // Calcular productos disponibles
            var availableStockCount = totalCount - lowStockCount - outOfStockCount;

            return new InventoryStatsDto
            {
                TotalProducts = totalCount,
                ActiveProductsCount = totalCount,
                InactiveProductsCount = 0, // TODO: Implementar cuando se agregue el filtro
                LowStockCount = lowStockCount,
                OutOfStockCount = outOfStockCount,
                AvailableStockCount = availableStockCount,
                TotalValue = totalValue,
                Currency = "USD",
                ValueTrendPercentage = 0, // TODO: Calcular tendencia
                ValueTrendDirection = "neutral"
            };
        }
    }
}
