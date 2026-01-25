using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class CreateProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public CreateProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Execute(Product product)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("El nombre del producto es requerido");

            if (product.Price.IsZeroOrNegative())
                throw new ArgumentException("El precio debe ser mayor a 0");

            // Crear producto
            product.Id = Guid.NewGuid().ToString();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            product.IsActive = true;

            return await _productRepository.CreateAsync(product);
        }
    }
}
