using System.Collections.Generic;
using System.Threading.Tasks;
using AutoPartesApp.Domain.Entities;

namespace AutoPartesApp.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAllInventoryAsync();
        Task<Inventory> GetByIdAsync(int id);
        Task<Inventory> GetByProductIdAsync(int productId);
        Task<Inventory> CreateAsync(Inventory inventory);
        Task<Inventory> UpdateAsync(Inventory inventory);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync(int threshold);
    }
}