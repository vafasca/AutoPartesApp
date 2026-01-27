using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesApp.Infrastructure.Persistence.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AutoPartesDbContext _context;

        public InventoryRepository(AutoPartesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoryAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Inventory> GetByIdAsync(int id)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Inventory> GetByProductIdAsync(int productId)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<Inventory> CreateAsync(Inventory inventory)
        {
            _ = _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Inventory> UpdateAsync(Inventory inventory)
        {
            _ = _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return false;
            }

            _ = _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Where(i => i.StockQuantity <= i.MinimumStock || i.StockQuantity <= threshold)
                .ToListAsync();
        }
    }
}