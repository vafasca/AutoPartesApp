using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class ToggleProductStatusUseCase
    {
        private readonly IProductRepository _productRepository;

        public ToggleProductStatusUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Execute(string productId)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(productId))
                throw new ArgumentException("El ID del producto es requerido");

            // Toggle status
            return await _productRepository.ToggleStatusAsync(productId);
        }
    }
}
