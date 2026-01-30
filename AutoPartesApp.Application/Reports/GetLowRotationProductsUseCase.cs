using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Reports
{
    public class GetLowRotationProductsUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public GetLowRotationProductsUseCase(
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<TopProductDto>> ExecuteAsync(
            ReportFilterDto filter,
            int minStock = 5,
            int maxUnitsSold = 10)
        {
            var dateTo = filter.DateTo ?? DateTime.UtcNow;
            var dateFrom = filter.DateFrom ?? dateTo.AddMonths(-3); // Últimos 3 meses por defecto

            // Obtener órdenes del período
            var orders = await _orderRepository.GetOrdersByDateRangeAsync(dateFrom, dateTo);

            // Productos vendidos en el período
            var soldProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.Product.Id)
                .Select(g => new
                {
                    ProductId = g.Key,
                    UnitsSold = g.Sum(oi => oi.Quantity)
                })
                .ToDictionary(x => x.ProductId, x => x.UnitsSold);

            // Obtener todos los productos activos
            var allProducts = await _productRepository.GetAllAsync();

            // Identificar productos con baja rotación
            var lowRotationProducts = allProducts
                .Where(p => p.Stock >= minStock) // Tiene stock
                .Select(p => new
                {
                    Product = p,
                    UnitsSold = soldProducts.ContainsKey(p.Id) ? soldProducts[p.Id] : 0
                })
                .Where(x => x.UnitsSold <= maxUnitsSold) // Pocas ventas
                .OrderBy(x => x.UnitsSold)
                .ThenByDescending(x => x.Product.Stock)
                .Take(20)
                .Select(x => new TopProductDto
                {
                    ProductId = x.Product.Id,
                    Name = x.Product.Name,
                    CategoryName = x.Product.Category?.Name ?? "Sin categoría",
                    UnitsSold = x.UnitsSold,
                    Revenue = x.UnitsSold * x.Product.Price.Amount,
                    ImageUrl = x.Product.ImageUrl,
                    Trend = "down",
                    TrendPercentage = 0
                })
                .ToList();

            return lowRotationProducts;
        }
    }
}
