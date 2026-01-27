using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPartesApp.Domain.Entities;
using AutoPartesApp.Shared.Services.Admin;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Inventory : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }
        
        [Inject]
        private InventoryService? InventoryService { get; set; }

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
            if (InventoryService == null)
            {
                Console.WriteLine("InventoryService no está disponible");
                return;
            }

            isLoading = true;
            StateHasChanged();

            try
            {
                await LoadInventoryStatsFromDb();
                await LoadFiltersFromDb();
                await LoadProductsFromDb();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadInventoryStatsFromDb()
        {
            if (InventoryService == null) return;

            var allProducts = await InventoryService.GetAllProductsAsync();
            var lowStockProducts = await InventoryService.GetLowStockProductsAsync(10);

            // Calcular valor total del inventario
            decimal totalValue = allProducts.Sum(p => p.Price.Amount * p.Stock);

            inventoryStats = new List<InventoryStat>
            {
                new InventoryStat
                {
                    Title = "Valor Total",
                    Value = totalValue.ToString("C"),
                    Icon = "trending_up",
                    TrendText = "+5.2%",
                    TrendColor = "text-emerald-500"
                },
                new InventoryStat
                {
                    Title = "Alertas Stock",
                    Value = $"{lowStockProducts.Count} items",
                    Icon = "warning",
                    TrendText = "Low stock",
                    TrendColor = "text-amber-500"
                }
            };
        }

        private async Task LoadFiltersFromDb()
        {
            if (InventoryService == null) return;

            var allProducts = await InventoryService.GetAllProductsAsync();
            
            var categories = allProducts.Select(p => p.Category.Name).Distinct().ToList();
            var lowStockCount = allProducts.Count(p => p.Stock <= 10 && p.Stock > 0);
            var outOfStockCount = allProducts.Count(p => p.Stock == 0);

            filters = new List<FilterOption>
            {
                new FilterOption { Label = "Todos", Value = "Todos", Count = allProducts.Count },
                new FilterOption { Label = "Bajo Stock", Value = "Bajo Stock", Count = lowStockCount },
                new FilterOption { Label = "Sin Stock", Value = "Sin Stock", Count = outOfStockCount }
            };

            // Añadir categorías únicas como filtros
            foreach (var category in categories)
            {
                var categoryCount = allProducts.Count(p => p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
                filters.Add(new FilterOption { Label = category, Value = category, Count = categoryCount });
            }
        }

        private async Task LoadProductsFromDb()
        {
            if (InventoryService == null) return;

            var products = await InventoryService.GetAllProductsAsync();

            // Convertir productos de la base de datos al modelo ProductItem
            allProducts = products.Select((p, index) => new ProductItem
            {
                Id = index + 1, // Usamos un índice incremental para evitar problemas con IDs no numéricos
                Name = p.Name,
                Sku = p.Sku,
                Price = p.Price.Amount,
                Stock = p.Stock,
                StockStatus = p.StockStatus, // Usamos la propiedad calculada
                Category = p.Category.Name,
                ImageUrl = p.ImageUrl ?? ""
            }).ToList();

            totalProducts = allProducts.Count;
            filteredProducts = new List<ProductItem>(allProducts);
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

        private async void HandleSearch()
        {
            await ApplyFiltersAndSearch();
        }

        private async void ApplyFilter(string filter)
        {
            selectedFilter = filter;
            await ApplyFiltersAndSearch();
        }

        private void ChangeView(string view)
        {
            currentView = view;
            StateHasChanged();
        }

        private async Task ApplyFiltersAndSearch()
        {
            if (InventoryService == null) return;

            IEnumerable<ProductItem> result = allProducts.AsEnumerable();

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

        private async void ClearFilters()
        {
            searchQuery = string.Empty;
            selectedFilter = "Todos";
            await ApplyFiltersAndSearch();
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
            if (status == "Sin stock" || stock == 0)
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
            // NavigationManager?.NavigateTo("/admin/inventory/new");
        }

        private void ImportProducts()
        {
            Console.WriteLine("📥 Importar productos");
        }

        private void ExportProducts()
        {
            Console.WriteLine("📤 Exportar productos");
        }

        private void ViewProductDetails(int productId)
        {
            Console.WriteLine($"👁️ Ver detalles del producto: {productId}");
            // NavigationManager?.NavigateTo($"/admin/inventory/{productId}");
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
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Sku { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public string StockStatus { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
        }
    }
}
