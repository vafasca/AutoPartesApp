using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoPartesApp.Domain.Entities;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;

        public InventoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/products");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                    return products ?? new List<Product>();
                }
                return new List<Product>();
            }
            catch
            {
                return new List<Product>();
            }
        }

        public async Task<List<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/low-stock?threshold={threshold}");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                    return products ?? new List<Product>();
                }
                return new List<Product>();
            }
            catch
            {
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Product>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/search?query={Uri.EscapeDataString(searchTerm ?? "")}");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                    return products ?? new List<Product>();
                }
                return new List<Product>();
            }
            catch
            {
                return new List<Product>();
            }
        }
    }
}
