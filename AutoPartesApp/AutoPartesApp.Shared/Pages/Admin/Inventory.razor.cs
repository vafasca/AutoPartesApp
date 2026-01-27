using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPartesApp.Shared.Services.Admin;
using AutoPartesApp.Shared.Models.Admin;
using AutoPartesApp.Application.DTOs;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Inventory : ComponentBase
    {
        [Inject]
        private InventoryService? InventoryService { get; set; }
        
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private bool hasNotifications = true;
        private bool isLoading = false;
        private string searchQuery = string.Empty;
        private string selectedFilter = "Todos";
        private string currentView = "grid"; // "grid" or "list"

        // Data
        private List<InventoryStat> inventoryStats = new();
        private List<FilterOption> filters = new();
        private List<ProductItem> allProducts = new();
        private List<ProductItem> filteredProducts = new();
        private int totalProducts = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            if (InventoryService == null) return;
            
            isLoading = true;
            StateHasChanged();

            try
            {
                var inventoryData = await InventoryService.GetInventoryDataAsync();
                
                // Convertir ProductDto a ProductItem
                allProducts = inventoryData.Products.Select(p => new ProductItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Price = p.Price,
                    Stock = p.Stock,
                    StockStatus = p.StockStatus,
                    Category = p.CategoryName,
                    ImageUrl = p.ImageUrl
                }).ToList();

                // Actualizar estadísticas
                inventoryStats = new List<InventoryStat>
                {
                    new InventoryStat
                    {
                        Title = "Valor Total",
                        Value = inventoryData.Stats.TotalValue.ToString("C"),
                        Icon = "trending_up",
                        TrendText = "+5.2%",
                        TrendColor = "text-emerald-500"
                    },
                    new InventoryStat
                    {
                        Title = "Productos Bajo Stock",
                        Value = $"{inventoryData.Stats.LowStockCount} items",
                        Icon = "warning",
                        TrendText = "Low stock",
                        TrendColor = "text-amber-500"
                    },
                    new InventoryStat
                    {
                        Title = "Productos Sin Stock",
                        Value = $"{inventoryData.Stats.OutOfStockCount} items",
                        Icon = "block",
                        TrendText = "Out of stock",
                        TrendColor = "text-red-500"
                    }
                };

                // Actualizar filtros
                LoadFilters(inventoryData);

                totalProducts = inventoryData.TotalProducts;
                filteredProducts = new List<ProductItem>(allProducts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading inventory data: {ex.Message}");
                // Manejar error de forma adecuada
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void LoadFilters(InventoryViewModel inventoryData)
        {
            var categoryCounts = allProducts.GroupBy(p => p.Category)
                                           .Select(g => new { Category = g.Key, Count = g.Count() })
                                           .ToList();

            filters = new List<FilterOption>
            {
                new FilterOption { Label = "Todos", Value = "Todos", Count = allProducts.Count },
                new FilterOption { Label = "Bajo Stock", Value = "Bajo Stock", Count = inventoryData.Stats.LowStockCount },
                new FilterOption { Label = "Sin Stock", Value = "Sin Stock", Count = inventoryData.Stats.OutOfStockCount }
            };

            // Añadir categorías únicas con conteos
            foreach (var category in categoryCounts)
            {
                filters.Add(new FilterOption 
                { 
                    Label = category.Category, 
                    Value = category.Category, 
                    Count = category.Count 
                });
            }
        }

        // Event Handlers
        private void ToggleNotifications()
        {
            hasNotifications = false;
            StateHasChanged();
        }

        private void ToggleMenu()
        {
            Console.WriteLine("Toggle menu");
        }

        private void ToggleFilters()
        {
            Console.WriteLine("Toggle filters modal");
        }

        private void HandleSearch()
        {
            ApplyFiltersAndSearch();
        }

        private void ApplyFilter(string filter)
        {
            selectedFilter = filter;
            ApplyFiltersAndSearch();
        }

        private void ChangeView(string view)
        {
            currentView = view;
            StateHasChanged();
        }

        private void ApplyFiltersAndSearch()
        {
            var result = allProducts.AsEnumerable();

            // Filtrar por categoría/estado
            if (selectedFilter != "Todos")
            {
                if (selectedFilter == "Bajo Stock")
                {
                    result = result.Where(p => p.Stock > 0 && p.Stock <= 10);
                }
                else if (selectedFilter == "Sin Stock")
                {
                    result = result.Where(p => p.Stock == 0);
                }
                else
                {
                    result = result.Where(p => p.Category.Equals(selectedFilter, StringComparison.OrdinalIgnoreCase));
                }
            }

            // Filtrar por búsqueda
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                result = result.Where(p =>
                    p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Sku.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Category.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                );
            }

            filteredProducts = result.ToList();
            StateHasChanged();
        }

        private void ClearFilters()
        {
            searchQuery = string.Empty;
            selectedFilter = "Todos";
            ApplyFiltersAndSearch();
        }

        // UI Helpers
        private string GetFilterPillClass(string filterValue)
        {
            var baseClass = "px-4 py-1.5 rounded-full text-xs font-bold whitespace-nowrap transition-all";
            if (selectedFilter == filterValue)
            {
                return $"{baseClass} bg-primary text-white";
            }
            return $"{baseClass} bg-gradient-to-br from-primary/10 to-primary/5 dark:from-primary/20 dark:to-primary/10 dark:bg-surface-dark text-slate-600 dark:text-slate-400 border border-slate-200 dark:border-border-dark hover:border-primary/50";
        }

        private string GetViewButtonClass(string view)
        {
            var baseClass = "h-9 w-9 flex items-center justify-center rounded-md transition-all";
            if (currentView == view)
            {
                return $"{baseClass} bg-gradient-to-br from-primary/10 to-primary/5 dark:from-primary/20 dark:to-primary/10 dark:bg-slate-700 shadow-sm";
            }
            return $"{baseClass} text-slate-500 dark:text-slate-400 hover:text-primary";
        }

        private string GetStockBadgeClass(string status, int stock)
        {
            if (status.Contains("Sin stock") || stock == 0)
            {
                return "text-xs font-extrabold text-red-500 bg-red-500/10 px-2 rounded-full";
            }
            else if (stock <= 10)
            {
                return "text-xs font-extrabold text-amber-500 bg-amber-500/10 px-2 rounded-full";
            }
            return "text-xs font-extrabold text-emerald-500 bg-emerald-500/10 px-2 rounded-full";
        }

        // Action Handlers
        private void AddNewProduct()
        {
            Console.WriteLine("📦 Agregar nuevo producto");
            NavigationManager?.NavigateTo("/admin/inventory/new");
        }

        private void ImportProducts()
        {
            Console.WriteLine("📥 Importar productos");
        }

        private void ExportProducts()
        {
            Console.WriteLine("📤 Exportar productos");
        }

        private void ViewProductDetails(string productId)
        {
            Console.WriteLine($"👁️ Ver detalles del producto: {productId}");
            NavigationManager?.NavigateTo($"/admin/inventory/{productId}");
        }

        // Data Models
        private class InventoryStat
        {
            public string Title { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string TrendText { get; set; } = string.Empty;
            public string TrendColor { get; set; } = string.Empty;
        }

        private class FilterOption
        {
            public string Label { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public int Count { get; set; }
        }

        private class ProductItem
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Sku { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public string StockStatus { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }
        }
    }
}
