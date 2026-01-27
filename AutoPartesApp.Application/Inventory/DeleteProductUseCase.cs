using System;
using System.Threading.Tasks;
using AutoPartesApp.Domain.Interfaces;

namespace AutoPartesApp.Application.Inventory
{
    public class DeleteProductUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public DeleteProductUseCase(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<bool> ExecuteAsync(int id)
        {
            // Implementación de eliminación lógica
            // En lugar de eliminar físicamente, podemos desactivar el item
            var inventoryItem = await _inventoryRepository.GetByIdAsync(id);
            
            if (inventoryItem == null)
            {
                return false;
            }

            // Realizar eliminación lógica cambiando el estado isActive
            inventoryItem.IsActive = false;
            inventoryItem.UpdatedAt = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventoryItem);
            return true;
        }
    }
}