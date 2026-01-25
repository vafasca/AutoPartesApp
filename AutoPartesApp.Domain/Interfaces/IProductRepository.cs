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
    }
}
