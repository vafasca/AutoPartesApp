using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class UpdateProductStockUseCase
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductStockUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Execute(UpdateStockDto dto)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(dto.ProductId))
                throw new ArgumentException("El ID del producto es requerido");

            if (dto.NewStock < 0)
                throw new ArgumentException("El stock no puede ser negativo");

            // Actualizar stock
            return await _productRepository.UpdateStockAsync(dto.ProductId, dto.NewStock);
        }
    }
}
