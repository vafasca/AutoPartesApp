using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Inventory
{
    public class GetLowStockUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetLowStockUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> Execute(int threshold = 10)
        {
            return await _productRepository.GetLowStockAsync(threshold);
        }
    }
}
