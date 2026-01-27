using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPartesApp.Shared.Services.Admin;
using AutoPartesApp.Shared.Models.Admin;

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
            if (InventoryService == null)
            {
                return;
            }
            
            isLoading = true;
            StateHasChanged();

            try
            {
                var inventoryList = await InventoryService.GetInventoryListAsync();
                
                // Convertir los datos del DTO a ProductItem
                allProducts = inventoryList.Items.Select(item => new ProductItem
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Name = item.ProductName,
                    Sku = $"SKU-{item.ProductId}", // Placeholder para SKU
                    Price = item.Price,
                    Stock = item.StockQuantity,
                    MinimumStock = item.MinimumStock,
                    StockStatus = item.IsLowStock ? "Bajo Stock" : (item.StockQuantity == 0 ? "Sin stock" : "Disponible"),
                    Category = item.CategoryName,
                    ImageUrl = item.ProductDescription.Length > 10 ? item.ProductDescription.Substring(0, 10) : "default-image.jpg", // Placeholder para imagen
                    IsActive = item.IsActive
                }).ToList();

                totalProducts = allProducts.Count;
                filteredProducts = new List<ProductItem>(allProducts);

                // Actualizar estadísticas
                UpdateInventoryStats(inventoryList);
                UpdateFilters();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading inventory data: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void UpdateInventoryStats(AutoPartesApp.Application.DTOs.InventoryDTOs.InventoryListDto inventoryList)
        {
            inventoryStats = new List<InventoryStat>
            {
                new InventoryStat
                {
                    Title = "Total Items",
                    Value = inventoryList.TotalItems.ToString(),
                    Icon = "inventory_2",
                    TrendText = $"{inventoryList.LowStockItemsCount} low stock",
                    TrendColor = inventoryList.LowStockItemsCount > 0 ? "text-amber-500" : "text-emerald-500"
                },
                new InventoryStat
                {
                    Title = "Low Stock Alerts",
                    Value = inventoryList.LowStockItemsCount.ToString(),
                    Icon = "warning",
                    TrendText = "items",
                    TrendColor = inventoryList.LowStockItemsCount > 0 ? "text-amber-500" : "text-emerald-500"
                },
                new InventoryStat
                {
                    Title = "Total Stock",
                    Value = inventoryList.TotalStock.ToString(),
                    Icon = "storage",
                    TrendText = "units",
                    TrendColor = "text-blue-500"
                }
            };
        }

        private void UpdateFilters()
        {
            filters = new List<FilterOption>
            {
                new FilterOption { Label = "Todos", Value = "Todos", Count = allProducts.Count },
                new FilterOption { Label = "Bajo Stock", Value = "Bajo Stock", Count = allProducts.Count(p => p.IsLowStock) },
                new FilterOption { Label = "Sin Stock", Value = "Sin Stock", Count = allProducts.Count(p => p.Stock == 0) },
                // Añadir filtros por categoría
                new FilterOption { Label = "Motor", Value = "Motor", Count = allProducts.Count(p => p.Category == "Motor") },
                new FilterOption { Label = "Frenos", Value = "Frenos", Count = allProducts.Count(p => p.Category == "Frenos") },
                new FilterOption { Label = "Suspensión", Value = "Suspensión", Count = allProducts.Count(p => p.Category == "Suspensión") }
            };
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
                    result = result.Where(p => p.IsLowStock && p.Stock > 0);
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

        private string GetStockBadgeClass(string status, int stock, int minimumStock)
        {
            if (stock == 0)
            {
                return "text-xs font-extrabold text-red-500 bg-red-500/10 px-2 rounded-full";
            }
            else if (stock <= minimumStock)
            {
                return "text-xs font-extrabold text-amber-500 bg-amber-500/10 px-2 rounded-full";
            }
            return "text-xs font-extrabold text-emerald-500 bg-emerald-500/10 px-2 rounded-full";
        }

        // Action Handlers
        private async Task AddNewProduct()
        {
            Console.WriteLine("📦 Agregar nuevo producto");
            // NavigationManager?.NavigateTo("/admin/inventory/new");
        }

        private async Task ImportProducts()
        {
            Console.WriteLine("📥 Importar productos");
        }

        private async Task ExportProducts()
        {
            Console.WriteLine("📤 Exportar productos");
        }

        private void ViewProductDetails(int productId)
        {
            Console.WriteLine($"👁️ Ver detalles del producto: {productId}");
            // NavigationManager?.NavigateTo($"/admin/inventory/{productId}");
        }

        private async Task UpdateStock(int productId, int quantityChange)
        {
            if (InventoryService == null)
            {
                return;
            }
            
            var updateDto = new AutoPartesApp.Application.DTOs.InventoryDTOs.UpdateStockDto
            {
                ProductId = productId,
                QuantityChange = quantityChange,
                Reason = "Manual adjustment"
            };

            var success = await InventoryService.UpdateStockAsync(updateDto);
            
            if (success)
            {
                await LoadData(); // Recargar los datos después de actualizar
            }
        }

        private async Task DeleteProduct(int id)
        {
            if (InventoryService == null)
            {
                return;
            }
            
            var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "¿Estás seguro de que deseas eliminar este producto?");
            
            if (confirmed)
            {
                var success = await InventoryService.DeleteProductAsync(id);
                
                if (success)
                {
                    await LoadData(); // Recargar los datos después de eliminar
                }
            }
        }
    }
}
