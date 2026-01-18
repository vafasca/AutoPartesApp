using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Inventory : ComponentBase
    {
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
            isLoading = true;
            StateHasChanged();

            await Task.Delay(500); // Simular carga

            LoadInventoryStats();
            LoadFilters();
            LoadProducts();

            isLoading = false;
            StateHasChanged();
        }

        private void LoadInventoryStats()
        {
            inventoryStats = new List<InventoryStat>
            {
                new InventoryStat
                {
                    Title = "Valor Total",
                    Value = "$452,000",
                    Icon = "trending_up",
                    TrendText = "+5.2%",
                    TrendColor = "text-emerald-500"
                },
                new InventoryStat
                {
                    Title = "Alertas Stock",
                    Value = "12 items",
                    Icon = "warning",
                    TrendText = "Low stock",
                    TrendColor = "text-amber-500"
                }
            };
        }

        private void LoadFilters()
        {
            filters = new List<FilterOption>
            {
                new FilterOption { Label = "Todos", Value = "Todos", Count = 0 },
                new FilterOption { Label = "Bajo Stock", Value = "Bajo Stock", Count = 3 },
                new FilterOption { Label = "Sin Stock", Value = "Sin Stock", Count = 1 },
                new FilterOption { Label = "Motor", Value = "Motor", Count = 4 }
            };
        }

        private void LoadProducts()
        {
            allProducts = new List<ProductItem>
            {
                new ProductItem
                {
                    Id = 1,
                    Name = "Filtro de Aceite Bosch",
                    Sku = "BO-44921-MT",
                    Price = 12.50m,
                    Stock = 5,
                    StockStatus = "Bajo Stock",
                    Category = "Motor",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAtiox9TJXUAiSseJQmSfXikwbJGHUEPi7N9wx0xwszHMjibwcpBLcCKJZoX8UU-tKGn78_MfENXELASdx92BT6uUbxcEVPH6kj3brP0-QBnBvvDx1WZD7M71arhjEu9oL-2_SMcXH8aklAdmgLQbTCimzVSgmn8SovX9aKFvV7AZqVlPVwEfKzs7U7OHgWkZV5XMGZ1_KgRRAZhDDl1llbR7IvJkQBzZUjAVPtGAgcpZ3FR_Efn-Xs_0RVXCMyXqZRBOu4tg5mWgM"
                },
                new ProductItem
                {
                    Id = 2,
                    Name = "Pastillas Freno Brembo",
                    Sku = "BR-99201-FR",
                    Price = 85.00m,
                    Stock = 24,
                    StockStatus = "Disponible",
                    Category = "Frenos",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCOz3tONHKIaAOG9OjRD-BhVcPZTtoGQu9tczLFBL-SaquR5SM7e3uEAzTGI8o579UNXA0CJGHzKz8BEbniV4TrjMva4JRZeS01_8ohm6zAMwkdPnanwdwNqIPcvhmHl73OGYXX_5IksWAJHNTzYiVpj5qHgfLGH4VrN1Lx07sSmrqmX5PMxjaiqY7ZSbOmHAzh1_dXKhMmydOm32lnVKBpQCagUL2TsvtgtrWMPX4eU1yjEp0UYAsX1S5lqCcnaG0qmZJQkgTtv8A"
                },
                new ProductItem
                {
                    Id = 3,
                    Name = "Bujías Iridium (4 pack)",
                    Sku = "NGK-7712-EL",
                    Price = 42.90m,
                    Stock = 0,
                    StockStatus = "Sin stock",
                    Category = "Eléctrico",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuA5ZwHq5SboWBdf1V4jV8rnureJKwvmHb_lOrx-kJ7WwiN37yHDETz8XnMZUF9mgTnjytBRanBT4sV_kgNW-yP01lVMzyldmd-Meavd8UYBl5-QKDKEqHYI9t_pLOveBdltGMyCULw3a8X6jjCFnposww9kgJyeqoX0nSIoQQ44szfFVIpqifcq33rHqu-bhX09P-YD7IQG-7MWb46tiKFhF8BJi20X_X3HCKiKBJHE2Mne6J61bnSCGF1MgD85aaWuURzmlECQlZs"
                },
                new ProductItem
                {
                    Id = 4,
                    Name = "Amortiguador Monroe",
                    Sku = "MN-2001-SP",
                    Price = 110.00m,
                    Stock = 18,
                    StockStatus = "Disponible",
                    Category = "Suspensión",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAkC65h6BdIXOtTFGHvyR4S-3G6YpdxTUP5Se_rVVKDrA4dEPtdN5kAOYjpFcesozvpN7g6pgi6K85dslj7Jw4F_ll_7RhFVM56FYKGoQ9333jOujlFWfzEC0YE6ud49XbausyZZxsgV32L4ecPyINnKUbpkgfnCYCHHCpzkGbn1T1xyNRTBMZH8XlZ2Gc_vxVhu6GtPrbnGW-9ES59YI1dYSPcCJgaPPmvU7SCPaRetG3az3D2O00YfQ1kgNSm4ozRKXO-9YidcFQ"
                },
                new ProductItem
                {
                    Id = 5,
                    Name = "Radiador Aluminio Toyota",
                    Sku = "TY-8821-MT",
                    Price = 135.00m,
                    Stock = 8,
                    StockStatus = "Bajo Stock",
                    Category = "Motor",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAtiox9TJXUAiSseJQmSfXikwbJGHUEPi7N9wx0xwszHMjibwcpBLcCKJZoX8UU-tKGn78_MfENXELASdx92BT6uUbxcEVPH6kj3brP0-QBnBvvDx1WZD7M71arhjEu9oL-2_SMcXH8aklAdmgLQbTCimzVSgmn8SovX9aKFvV7AZqVlPVwEfKzs7U7OHgWkZV5XMGZ1_KgRRAZhDDl1llbR7IvJkQBzZUjAVPtGAgcpZ3FR_Efn-Xs_0RVXCMyXqZRBOu4tg5mWgM"
                },
                new ProductItem
                {
                    Id = 6,
                    Name = "Kit Distribución Completo",
                    Sku = "GT-5501-MT",
                    Price = 245.00m,
                    Stock = 12,
                    StockStatus = "Disponible",
                    Category = "Motor",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCOz3tONHKIaAOG9OjRD-BhVcPZTtoGQu9tczLFBL-SaquR5SM7e3uEAzTGI8o579UNXA0CJGHzKz8BEbniV4TrjMva4JRZeS01_8ohm6zAMwkdPnanwdwNqIPcvhmHl73OGYXX_5IksWAJHNTzYiVpj5qHgfLGH4VrN1Lx07sSmrqmX5PMxjaiqY7ZSbOmHAzh1_dXKhMmydOm32lnVKBpQCagUL2TsvtgtrWMPX4eU1yjEp0UYAsX1S5lqCcnaG0qmZJQkgTtv8A"
                }
            };

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
            return $"{baseClass} bg-white dark:bg-surface-dark text-slate-600 dark:text-slate-400 border border-slate-200 dark:border-border-dark hover:border-primary/50";
        }

        private string GetViewButtonClass(string view)
        {
            var baseClass = "h-9 w-9 flex items-center justify-center rounded-md transition-all";
            if (currentView == view)
            {
                return $"{baseClass} bg-white dark:bg-slate-700 shadow-sm";
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
