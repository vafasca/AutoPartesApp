using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Threading.Tasks;

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
            //Ejecutar queries SECUENCIALMENTE
            var totalCount = await _productRepository.GetTotalCountAsync();
            var totalValue = await _productRepository.GetTotalValueAsync();
            var lowStockCount = await _productRepository.GetLowStockCountAsync(10);
            var outOfStockCount = await _productRepository.GetOutOfStockCountAsync();

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