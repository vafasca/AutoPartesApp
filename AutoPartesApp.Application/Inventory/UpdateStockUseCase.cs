using System;
using System.Threading.Tasks;
using AutoPartesApp.Application.DTOs.InventoryDTOs;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;

namespace AutoPartesApp.Application.Inventory
{
    public class UpdateStockUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public UpdateStockUseCase(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<bool> ExecuteAsync(UpdateStockDto updateStockDto)
        {
            var inventoryItem = await _inventoryRepository.GetByProductIdAsync(updateStockDto.ProductId);
            
            if (inventoryItem == null)
            {
                // Si no existe, crear un nuevo registro de inventario
                var newInventory = new Inventory
                {
                    ProductId = updateStockDto.ProductId,
                    StockQuantity = Math.Max(0, updateStockDto.QuantityChange), // No permitir stock negativo en creación
                    MinimumStock = 5, // Valor por defecto
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _inventoryRepository.CreateAsync(newInventory);
                return true;
            }

            // Actualizar el stock existente
            var newStock = inventoryItem.StockQuantity + updateStockDto.QuantityChange;
            
            // Validar que no sea negativo
            if (newStock < 0)
            {
                throw new InvalidOperationException("No se puede actualizar el stock a un valor negativo.");
            }

            inventoryItem.StockQuantity = newStock;
            inventoryItem.UpdatedAt = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventoryItem);
            return true;
        }
    }
}