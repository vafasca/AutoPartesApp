using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoPartesApp.Shared.Models.Admin;
using Newtonsoft.Json;

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
            var response = await _httpClient.GetAsync("api/v1.0/admin/inventory");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<ProductDto>>(json);
            
            // Obtener estadísticas
            var statsResponse = await _httpClient.GetAsync("api/v1.0/admin/inventory/stats");
            statsResponse.EnsureSuccessStatusCode();
            
            var statsJson = await statsResponse.Content.ReadAsStringAsync();
            dynamic statsData = JsonConvert.DeserializeObject(statsJson);
            
            var viewModel = new InventoryViewModel
            {
                Products = products ?? new List<ProductDto>(),
                TotalProducts = (int)statsData.totalProducts,
                Stats = new InventoryStats
                {
                    TotalProducts = (int)statsData.totalProducts,
                    TotalValue = (decimal)statsData.totalValue,
                    LowStockCount = (int)statsData.lowStockCount,
                    OutOfStockCount = (int)statsData.outOfStockCount
                }
            };
            
            return viewModel;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/v1.0/admin/inventory");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ProductDto>>(json) ?? new List<ProductDto>();
        }

        public async Task<List<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
        {
            var response = await _httpClient.GetAsync($"api/v1.0/admin/inventory/low-stock?threshold={threshold}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ProductDto>>(json) ?? new List<ProductDto>();
        }

        public async Task<ProductDto> GetProductByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"api/v1.0/admin/inventory/{id}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductDto>(json);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto product)
        {
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/v1.0/admin/inventory", content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductDto>(responseJson);
        }

        public async Task<ProductDto> UpdateProductAsync(string id, ProductDto product)
        {
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/v1.0/admin/inventory/{id}", content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductDto>(responseJson);
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/v1.0/admin/inventory/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
