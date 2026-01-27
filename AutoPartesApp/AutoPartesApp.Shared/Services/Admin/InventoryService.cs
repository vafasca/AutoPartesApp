using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Core.Application.DTOs.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class InventoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InventoryService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public InventoryService(HttpClient httpClient, ILogger<InventoryService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Obtener productos paginados con filtros
        /// </summary>
        public async Task<PagedResultDto<ProductListItemDto>?> GetProductsAsync(ProductFilterDto filter)
        {
            try
            {
                var queryParams = BuildQueryString(filter);
                var response = await _httpClient.GetAsync($"api/products?{queryParams}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PagedResultDto<ProductListItemDto>>(_jsonOptions);
                }

                _logger.LogError($"Error al obtener productos: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return null;
            }
        }

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        public async Task<ProductDto?> GetProductByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);
                }

                _logger.LogError($"Error al obtener producto {id}: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener producto {id}");
                return null;
            }
        }

        /// <summary>
        /// Crear nuevo producto
        /// </summary>
        public async Task<ProductDto?> CreateProductAsync(CreateProductDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/products", dto);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error al crear producto: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return null;
            }
        }

        /// <summary>
        /// Actualizar producto existente
        /// </summary>
        public async Task<ProductDto?> UpdateProductAsync(string id, UpdateProductDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", dto);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error al actualizar producto {id}: {response.StatusCode} - {errorContent}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar producto {id}");
                return null;
            }
        }

        /// <summary>
        /// Actualizar stock de un producto
        /// </summary>
        public async Task<bool> UpdateStockAsync(string id, int newStock)
        {
            try
            {
                var dto = new UpdateStockDto
                {
                    ProductId = id,
                    NewStock = newStock
                };

                var response = await _httpClient.PatchAsJsonAsync($"api/products/{id}/stock", dto);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error al actualizar stock del producto {id}: {response.StatusCode} - {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar stock del producto {id}");
                return false;
            }
        }

        /// <summary>
        /// Activar/Desactivar producto
        /// </summary>
        public async Task<bool> ToggleProductStatusAsync(string id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/products/{id}/toggle-status", null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error al cambiar estado del producto {id}: {response.StatusCode} - {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cambiar estado del producto {id}");
                return false;
            }
        }

        /// <summary>
        /// Buscar productos
        /// </summary>
        public async Task<List<ProductListItemDto>?> SearchProductsAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/search?query={Uri.EscapeDataString(query)}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>(_jsonOptions);
                }

                _logger.LogError($"Error al buscar productos: {response.StatusCode}");
                return new List<ProductListItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos");
                return new List<ProductListItemDto>();
            }
        }

        /// <summary>
        /// Obtener productos con stock bajo
        /// </summary>
        public async Task<List<ProductListItemDto>?> GetLowStockProductsAsync(int threshold = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/products/low-stock?threshold={threshold}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>(_jsonOptions);
                }

                _logger.LogError($"Error al obtener productos con stock bajo: {response.StatusCode}");
                return new List<ProductListItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos con stock bajo");
                return new List<ProductListItemDto>();
            }
        }

        /// <summary>
        /// Obtener estadísticas del inventario
        /// </summary>
        public async Task<InventoryStatsDto?> GetInventoryStatsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/inventory/stats");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<InventoryStatsDto>(_jsonOptions);
                }

                _logger.LogError($"Error al obtener estadísticas del inventario: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas del inventario");
                return null;
            }
        }

        /// <summary>
        /// Construir query string para filtros
        /// </summary>
        private string BuildQueryString(ProductFilterDto filter)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
                queryParams.Add($"searchQuery={Uri.EscapeDataString(filter.SearchQuery)}");

            if (!string.IsNullOrWhiteSpace(filter.CategoryId))
                queryParams.Add($"categoryId={Uri.EscapeDataString(filter.CategoryId)}");

            if (!string.IsNullOrWhiteSpace(filter.StockStatus))
                queryParams.Add($"stockStatus={Uri.EscapeDataString(filter.StockStatus)}");

            if (filter.MinStock.HasValue)
                queryParams.Add($"minStock={filter.MinStock.Value}");

            if (filter.MaxStock.HasValue)
                queryParams.Add($"maxStock={filter.MaxStock.Value}");

            if (filter.IsActive.HasValue)
                queryParams.Add($"isActive={filter.IsActive.Value}");

            queryParams.Add($"pageNumber={filter.PageNumber}");
            queryParams.Add($"pageSize={filter.PageSize}");

            if (!string.IsNullOrWhiteSpace(filter.SortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(filter.SortBy)}");

            if (!string.IsNullOrWhiteSpace(filter.SortOrder))
                queryParams.Add($"sortOrder={Uri.EscapeDataString(filter.SortOrder)}");

            return string.Join("&", queryParams);
        }
    }
}
