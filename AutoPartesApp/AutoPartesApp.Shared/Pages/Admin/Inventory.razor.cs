using AutoPartesApp.Shared.Models.Admin;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Inventory : ComponentBase
    {
        [Inject]
        private InventoryService? InventoryService { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // ViewModel
        private InventoryViewModel viewModel = null!;

        // Estado local
        private bool hasNotifications = true;
        private bool isLoading = false;
        private string searchQuery = string.Empty;
        private string selectedFilter = "Todos";
        private string currentView = "grid"; // "grid" or "list"

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (InventoryService == null)
                {
                    Console.WriteLine("❌ InventoryService no está inyectado");
                    return;
                }

                // Inicializar ViewModel
                viewModel = new InventoryViewModel(InventoryService);

                // Cargar datos
                await LoadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                await viewModel.LoadDataAsync();

                // Sincronizar estado local con ViewModel
                searchQuery = viewModel.SearchQuery;
                selectedFilter = viewModel.SelectedFilter;
                currentView = viewModel.CurrentView;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar datos: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        // ========== EVENT HANDLERS ==========

        private void ToggleNotifications()
        {
            hasNotifications = false;
            StateHasChanged();
        }

        private void ToggleMenu()
        {
            Console.WriteLine("🔧 Toggle menu");
        }

        private void ToggleFilters()
        {
            Console.WriteLine("🔧 Toggle filters modal");
        }

        private async Task HandleSearch()
        {
            try
            {
                viewModel.SearchQuery = searchQuery;
                await viewModel.SearchAsync(searchQuery);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al buscar: {ex.Message}");
            }
        }

        private async Task ApplyFilter(string filter)
        {
            try
            {
                selectedFilter = filter;
                await viewModel.ApplyFilterAsync(filter);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al aplicar filtro: {ex.Message}");
            }
        }

        private void ChangeView(string view)
        {
            currentView = view;
            viewModel.ChangeView(view);
            StateHasChanged();
        }

        private async Task ClearFilters()
        {
            try
            {
                searchQuery = string.Empty;
                selectedFilter = "Todos";
                await viewModel.ClearFiltersAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al limpiar filtros: {ex.Message}");
            }
        }

        private async Task ChangePage(int page)
        {
            try
            {
                await viewModel.ChangePageAsync(page);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cambiar página: {ex.Message}");
            }
        }

        // ========== UI HELPERS ==========

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
            if (status == "Sin stock" || status == "Agotado" || stock == 0)
            {
                return "text-xs font-extrabold text-red-500 bg-red-500/10 px-2 rounded-full";
            }
            else if (stock <= 10 || status == "Bajo Stock")
            {
                return "text-xs font-extrabold text-amber-500 bg-amber-500/10 px-2 rounded-full";
            }
            return "text-xs font-extrabold text-emerald-500 bg-emerald-500/10 px-2 rounded-full";
        }

        // ========== ACTION HANDLERS ==========

        private void AddNewProduct()
        {
            Console.WriteLine("📦 Agregar nuevo producto");
            NavigationManager?.NavigateTo("/admin/inventory/new");
        }

        private void ImportProducts()
        {
            Console.WriteLine("📥 Importar productos");
            // TODO: Implementar modal de importación
        }

        private void ExportProducts()
        {
            Console.WriteLine("📤 Exportar productos");
            // TODO: Implementar exportación
        }

        private void ViewProductDetails(string productId)
        {
            Console.WriteLine($"👁️ Ver detalles del producto: {productId}");
            NavigationManager?.NavigateTo($"/admin/inventory/{productId}");
        }
    }
}
