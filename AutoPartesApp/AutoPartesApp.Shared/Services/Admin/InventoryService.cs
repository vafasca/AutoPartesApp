using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoPartesApp.Shared.Models.Admin;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;

        public InventoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InventoryViewModel> GetInventoryDataAsync()
        {
            var response = await _httpClient.GetAsync("api/inventory");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var products = JsonSerializer.Deserialize<List<ProductDto>>(json, options);
            
            // Obtener estadísticas
            var statsResponse = await _httpClient.GetAsync("api/inventory/stats");
            statsResponse.EnsureSuccessStatusCode();
            
            var statsJson = await statsResponse.Content.ReadAsStringAsync();
            var statsOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var statsData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(statsJson, statsOptions);
            
            var viewModel = new InventoryViewModel
            {
                Products = products ?? new List<ProductDto>(),
                TotalProducts = statsData.ContainsKey("totalProducts") ? statsData["totalProducts"].GetInt32() : 0,
                Stats = new InventoryStats
                {
                    TotalProducts = statsData.ContainsKey("totalProducts") ? statsData["totalProducts"].GetInt32() : 0,
                    TotalValue = statsData.ContainsKey("totalValue") ? statsData["totalValue"].GetDecimal() : 0,
                    LowStockCount = statsData.ContainsKey("lowStockCount") ? statsData["lowStockCount"].GetInt32() : 0,
                    OutOfStockCount = statsData.ContainsKey("outOfStockCount") ? statsData["outOfStockCount"].GetInt32() : 0
                }
            };
            
            return viewModel;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/inventory");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<ProductDto>>(json, options) ?? new List<ProductDto>();
        }

        public async Task<List<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
        {
            var response = await _httpClient.GetAsync($"api/inventory/low-stock?threshold={threshold}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<ProductDto>>(json, options) ?? new List<ProductDto>();
        }

        public async Task<ProductDto> GetProductByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/inventory/{id}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ProductDto>(json, options);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto product)
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/inventory", content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ProductDto>(responseJson, options);
        }

        public async Task<ProductDto> UpdateProductAsync(string id, ProductDto product)
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/inventory/{id}", content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ProductDto>(responseJson, options);
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/inventory/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
