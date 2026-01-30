using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AutoPartesDbContext _context;

        public ProductRepository(AutoPartesDbContext context)
        {
            _context = context;
        }

        // ========== MÉTODOS EXISTENTES ==========

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetLowStockAsync(int threshold = 10)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Stock > 0 && p.Stock <= threshold && p.IsActive)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // MÉTODOS EXISTENTES AGREGADOS ANTERIORMENTE

        public async Task<(List<Product> Products, int TotalCount)> GetPagedAsync(
            string? searchQuery,
            string? categoryId,
            string? stockStatus,
            int? minStock,
            int? maxStock,
            bool? isActive,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchQuery) ||
                    p.Sku.Contains(searchQuery) ||
                    p.Category.Name.Contains(searchQuery) ||
                    p.Description.Contains(searchQuery));
            }

            // Filtro por categoría
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Filtro por estado de stock
            if (!string.IsNullOrWhiteSpace(stockStatus))
            {
                switch (stockStatus.ToLower())
                {
                    case "disponible":
                        query = query.Where(p => p.Stock > 10);
                        break;
                    case "bajo stock":
                        query = query.Where(p => p.Stock > 0 && p.Stock <= 10);
                        break;
                    case "agotado":
                    case "sin stock":
                        query = query.Where(p => p.Stock == 0);
                        break;
                }
            }

            // Filtro por rango de stock
            if (minStock.HasValue)
            {
                query = query.Where(p => p.Stock >= minStock.Value);
            }
            if (maxStock.HasValue)
            {
                query = query.Where(p => p.Stock <= maxStock.Value);
            }

            // Filtro por estado activo
            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }
            else
            {
                // Por defecto, solo mostrar activos
                query = query.Where(p => p.IsActive);
            }

            // Contar total
            var totalCount = await query.CountAsync();

            // Aplicar paginación
            var products = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .CountAsync();
        }

        public async Task<decimal> GetTotalValueAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();

            return products.Sum(p => p.Price.Amount * p.Stock);
        }

        public async Task<int> GetOutOfStockCountAsync()
        {
            return await _context.Products
                .Where(p => p.Stock == 0 && p.IsActive)
                .CountAsync();
        }

        public async Task<int> GetLowStockCountAsync(int threshold = 10)
        {
            return await _context.Products
                .Where(p => p.Stock > 0 && p.Stock <= threshold && p.IsActive)
                .CountAsync();
        }

        public async Task<bool> UpdateStockAsync(string productId, int newStock)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.Stock = newStock;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(string productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.IsActive = !product.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> SearchAsync(string query)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && (
                    p.Name.Contains(query) ||
                    p.Sku.Contains(query) ||
                    p.Category.Name.Contains(query) ||
                    p.Description.Contains(query)))
                .Take(20) // Limitar a 20 resultados
                .ToListAsync();
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Sku == sku);
        }

        // ========== MÉTODOS NUEVOS PARA REPORTES DE INVENTARIO ==========

        public async Task<List<Product>> GetLowRotationProductsAsync(
            DateTime dateFrom,
            DateTime dateTo,
            int maxSales = 5)
        {
            // Obtener IDs de productos vendidos en el período
            var productSales = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .Where(x => x.TotalSold <= maxSales)
                .Select(x => x.ProductId)
                .ToListAsync();

            // Obtener productos con baja rotación
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && productSales.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<List<(string CategoryId, string CategoryName, decimal TotalValue, int TotalUnits)>>
            GetInventoryValueByCategoryAsync()
        {
            var result = await _context.Products
                .Where(p => p.IsActive)
                .GroupBy(p => new
                {
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalValue = g.Sum(p => p.Price.Amount * p.Stock),
                    TotalUnits = g.Sum(p => p.Stock)
                })
                .OrderByDescending(x => x.TotalValue)
                .ToListAsync();

            return result.Select(x => (
                x.CategoryId,
                x.CategoryName,
                x.TotalValue,
                x.TotalUnits
            )).ToList();
        }

        public async Task<List<(string ProductId, string ProductName, int TotalSold, decimal Revenue)>>
            GetMostMovedProductsAsync(
                DateTime dateFrom,
                DateTime dateTo,
                int topN = 10)
        {
            var result = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .SelectMany(o => o.Items)
                .GroupBy(oi => new
                {
                    oi.Product.Id,
                    oi.Product.Name
                })
                .Select(g => new
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.Name,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(topN)
                .ToListAsync();

            return result.Select(x => (
                x.ProductId,
                x.ProductName,
                x.TotalSold,
                x.Revenue
            )).ToList();
        }

        public async Task<List<Product>> GetProductsWithoutMovementAsync(
            DateTime dateFrom,
            DateTime dateTo)
        {
            // Obtener IDs de productos vendidos en el período
            var soldProductIds = await _context.Orders
                .Where(o => o.CreatedAt >= dateFrom && o.CreatedAt <= dateTo)
                .SelectMany(o => o.Items)
                .Select(oi => oi.ProductId)
                .Distinct()
                .ToListAsync();

            // Obtener productos que NO se vendieron
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && !soldProductIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task<List<(string CategoryName, int TotalProducts, int InStock, int OutOfStock, int LowStock)>>
            GetStockStatsByCategoryAsync()
        {
            var result = await _context.Products
                .Where(p => p.IsActive)
                .GroupBy(p => p.Category.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalProducts = g.Count(),
                    InStock = g.Count(p => p.Stock > 10),
                    OutOfStock = g.Count(p => p.Stock == 0),
                    LowStock = g.Count(p => p.Stock > 0 && p.Stock <= 10)
                })
                .OrderBy(x => x.CategoryName)
                .ToListAsync();

            return result.Select(x => (
                x.CategoryName,
                x.TotalProducts,
                x.InStock,
                x.OutOfStock,
                x.LowStock
            )).ToList();
        }
    }
}
