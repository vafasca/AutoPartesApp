using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Application.Inventory
{
    public class UpdateProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> Execute(string productId, Product updatedProduct)
        {
            var existingProduct = await _productRepository.GetByIdAsync(productId);

            if (existingProduct == null)
                return null;

            // Actualizar propiedades
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.CategoryId = updatedProduct.CategoryId;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            return await _productRepository.UpdateAsync(existingProduct);
        }
    }
}
