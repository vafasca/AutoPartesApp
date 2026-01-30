using AutoPartesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(string id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetLowStockAsync(int threshold = 10);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(string id);

        Task<(List<Product> Products, int TotalCount)> GetPagedAsync(
            string? searchQuery,
            string? categoryId,
            string? stockStatus,
            int? minStock,
            int? maxStock,
            bool? isActive,
            int pageNumber,
            int pageSize);

        Task<int> GetTotalCountAsync();
        Task<decimal> GetTotalValueAsync();
        Task<int> GetOutOfStockCountAsync();
        Task<int> GetLowStockCountAsync(int threshold = 10);
        Task<bool> UpdateStockAsync(string productId, int newStock);
        Task<bool> ToggleStatusAsync(string productId);
        Task<List<Product>> SearchAsync(string query);
        Task<Product?> GetBySkuAsync(string sku);

        /// <summary>
        /// Obtener productos con baja rotación (pocas ventas en el período)
        /// </summary>
        Task<List<Product>> GetLowRotationProductsAsync(
            DateTime dateFrom,
            DateTime dateTo,
            int maxSales = 5);

        /// <summary>
        /// Obtener valoración de inventario agrupada por categoría
        /// </summary>
        Task<List<(string CategoryId, string CategoryName, decimal TotalValue, int TotalUnits)>>
            GetInventoryValueByCategoryAsync();

        /// <summary>
        /// Obtener productos más movidos (más vendidos) en un período
        /// </summary>
        Task<List<(string ProductId, string ProductName, int TotalSold, decimal Revenue)>>
            GetMostMovedProductsAsync(
                DateTime dateFrom,
                DateTime dateTo,
                int topN = 10);

        /// <summary>
        /// Obtener productos sin movimiento en un período
        /// </summary>
        Task<List<Product>> GetProductsWithoutMovementAsync(
            DateTime dateFrom,
            DateTime dateTo);

        /// <summary>
        /// Obtener estadísticas de stock por categoría
        /// </summary>
        Task<List<(string CategoryName, int TotalProducts, int InStock, int OutOfStock, int LowStock)>>
            GetStockStatsByCategoryAsync();
    }
}
