using AutoPartesApp.Core.Application.DTOs;
using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Core.Application.DTOs.Common;
using AutoPartesApp.Shared.Services.Admin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoPartesApp.Shared.Models.Admin
{
    public class InventoryViewModel
    {
        private readonly InventoryService _inventoryService;

        // ========== STATE ==========
        public bool IsLoading { get; set; }
        public bool HasNotifications { get; set; } = true;
        public string SearchQuery { get; set; } = string.Empty;
        public string SelectedFilter { get; set; } = "Todos";
        public string CurrentView { get; set; } = "grid"; // "grid" o "list"

        // ========== DATA ==========
        public List<InventoryStat> InventoryStats { get; set; } = new();
        public List<FilterOption> Filters { get; set; } = new();
        public List<ProductListItemDto> FilteredProducts { get; set; } = new();
        public PagedResultDto<ProductListItemDto>? PagedResult { get; set; }

        // ========== PAGINATION ==========
        public int TotalProducts { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }

        // ========== FILTER OPTIONS ==========
        public List<string> Categories { get; set; } = new();
        public List<string> StockStatuses { get; set; } = new()
        {
            "Todos",
            "Disponible",
            "Bajo Stock",
            "Agotado"
        };

        // Constructor
        public InventoryViewModel(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Constructor sin parámetros (para casos donde se inyecta después)
        public InventoryViewModel()
        {
            _inventoryService = null!;
        }

        /// <summary>
        /// Cargar todos los datos iniciales
        /// </summary>
        public async Task LoadDataAsync()
        {
            IsLoading = true;

            await Task.WhenAll(
                LoadStatsAsync(),
                LoadFiltersAsync(),
                LoadProductsAsync()
            );

            IsLoading = false;
        }

        /// <summary>
        /// Cargar estadísticas del inventario
        /// </summary>
        public async Task LoadStatsAsync()
        {
            var stats = await _inventoryService.GetInventoryStatsAsync();

            if (stats != null)
            {
                InventoryStats = new List<InventoryStat>
                {
                    new InventoryStat
                    {
                        Title = "Valor Total",
                        Value = stats.FormattedTotalValue,
                        Icon = "trending_up",
                        TrendText = stats.ValueTrendPercentage > 0 ? $"+{stats.ValueTrendPercentage:F1}%" : $"{stats.ValueTrendPercentage:F1}%",
                        TrendColor = stats.ValueTrendPercentage > 0 ? "text-emerald-500" :
                                    stats.ValueTrendPercentage < 0 ? "text-red-500" : "text-slate-500"
                    },
                    new InventoryStat
                    {
                        Title = "Alertas Stock",
                        Value = stats.LowStockAlertText,
                        Icon = "warning",
                        TrendText = stats.LowStockCount > 0 ? "Low stock" : "Sin alertas",
                        TrendColor = stats.LowStockCount > 0 ? "text-amber-500" : "text-emerald-500"
                    }
                };
            }
        }

        /// <summary>
        /// Cargar opciones de filtros
        /// </summary>
        public async Task LoadFiltersAsync()
        {
            // Cargar estadísticas para los contadores de filtros
            var stats = await _inventoryService.GetInventoryStatsAsync();

            Filters = new List<FilterOption>
            {
                new FilterOption { Label = "Todos", Value = "Todos", Count = stats?.TotalProducts ?? 0 },
                new FilterOption { Label = "Bajo Stock", Value = "Bajo Stock", Count = stats?.LowStockCount ?? 0 },
                new FilterOption { Label = "Sin Stock", Value = "Sin Stock", Count = stats?.OutOfStockCount ?? 0 }
            };
        }

        /// <summary>
        /// Cargar productos con filtros actuales
        /// </summary>
        public async Task LoadProductsAsync()
        {
            var filter = BuildFilter();
            PagedResult = await _inventoryService.GetProductsAsync(filter);

            if (PagedResult != null)
            {
                FilteredProducts = PagedResult.Items;
                TotalProducts = PagedResult.TotalCount;
                CurrentPage = PagedResult.PageNumber;
                TotalPages = PagedResult.TotalPages;
            }
            else
            {
                FilteredProducts = new List<ProductListItemDto>();
                TotalProducts = 0;
                TotalPages = 0;
            }
        }

        /// <summary>
        /// Aplicar filtros y recargar productos
        /// </summary>
        public async Task ApplyFiltersAsync()
        {
            CurrentPage = 1; // Reset a la primera página
            await LoadProductsAsync();
        }

        /// <summary>
        /// Buscar productos por query
        /// </summary>
        public async Task SearchAsync(string query)
        {
            SearchQuery = query;
            CurrentPage = 1; // Reset a la primera página
            await LoadProductsAsync();
        }

        /// <summary>
        /// Cambiar de página
        /// </summary>
        public async Task ChangePageAsync(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
            await LoadProductsAsync();
        }

        /// <summary>
        /// Cambiar vista (grid/list)
        /// </summary>
        public void ChangeView(string view)
        {
            if (view == "grid" || view == "list")
            {
                CurrentView = view;
            }
        }

        /// <summary>
        /// Aplicar filtro por estado o categoría
        /// </summary>
        public async Task ApplyFilterAsync(string filterValue)
        {
            SelectedFilter = filterValue;
            await ApplyFiltersAsync();
        }

        /// <summary>
        /// Limpiar todos los filtros
        /// </summary>
        public async Task ClearFiltersAsync()
        {
            SearchQuery = string.Empty;
            SelectedFilter = "Todos";
            CurrentPage = 1;
            await LoadProductsAsync();
        }

        /// <summary>
        /// Construir objeto de filtro para la API
        /// </summary>
        private ProductFilterDto BuildFilter()
        {
            var filter = new ProductFilterDto
            {
                SearchQuery = string.IsNullOrWhiteSpace(SearchQuery) ? null : SearchQuery,
                PageNumber = CurrentPage,
                PageSize = PageSize
            };

            // Aplicar filtro por estado de stock
            if (SelectedFilter != "Todos")
            {
                if (SelectedFilter == "Bajo Stock" || SelectedFilter == "Sin Stock" || SelectedFilter == "Disponible")
                {
                    filter.StockStatus = SelectedFilter;
                }
                else
                {
                    // Es un filtro de categoría
                    filter.CategoryId = SelectedFilter;
                }
            }

            return filter;
        }

        /// <summary>
        /// Refrescar datos (útil después de crear/actualizar/eliminar)
        /// </summary>
        public async Task RefreshAsync()
        {
            await LoadDataAsync();
        }
    }
}