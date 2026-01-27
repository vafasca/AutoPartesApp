using AutoPartesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IProductRepository
    {
        // Existentes
        Task<Product?> GetByIdAsync(string id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetLowStockAsync(int threshold = 10);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(string id);

        // NUEVOS
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
    }
}
