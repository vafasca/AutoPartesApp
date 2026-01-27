using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoPartesApp.Application.DTOs.InventoryDTOs;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;

        public InventoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InventoryListDto> GetInventoryListAsync()
        {
            var response = await _httpClient.GetAsync("api/v1/admin/inventory/list");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                return JsonSerializer.Deserialize<InventoryListDto>(jsonContent, options);
            }
            
            throw new HttpRequestException($"Error al obtener el inventario: {response.StatusCode}");
        }

        public async Task<bool> UpdateStockAsync(UpdateStockDto updateStockDto)
        {
            var json = JsonSerializer.Serialize(updateStockDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/v1/admin/inventory/update-stock", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/v1/admin/inventory/{id}");
            
            return response.IsSuccessStatusCode;
        }
    }
}
