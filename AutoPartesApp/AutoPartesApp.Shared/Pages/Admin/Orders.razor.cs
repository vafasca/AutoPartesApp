using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Orders : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private bool isLoading = false;
        private bool hasNotifications = true;
        private bool showSearch = false;
        private bool isDesktop = false;
        private bool showAssignDriverModal = false;
        private string searchQuery = string.Empty;
        private string selectedFilter = "Todos";
        private int activeDriversCount = 4;
        private int selectedOrderId = 0;

        // Data
        private List<FilterOption> filters = new();
        private List<OrderItem> allOrders = new();
        private List<OrderItem> filteredOrders = new();
        private List<DriverOption> availableDrivers = new();
        private int totalOrders = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            CheckViewport();
        }

        private async Task LoadData()
        {
            isLoading = true;
            StateHasChanged();

            await Task.Delay(500); // Simular carga

            LoadFilters();
            LoadOrders();
            LoadAvailableDrivers();

            isLoading = false;
            StateHasChanged();
        }

        private void CheckViewport()
        {
            // En producción, usar JS Interop para detectar el viewport
            isDesktop = false; // Por defecto mobile
        }

        private void LoadFilters()
        {
            filters = new List<FilterOption>
            {
                new FilterOption { Label = "Todos", Value = "Todos" },
                new FilterOption { Label = "Pendiente", Value = "Pendiente" },
                new FilterOption { Label = "En Ruta", Value = "En Ruta" },
                new FilterOption { Label = "Entregado", Value = "Entregado" }
            };
        }

        private void LoadOrders()
        {
            allOrders = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = 1,
                    OrderNumber = "#4521",
                    ProductName = "Kit de Embrague (Clutch)",
                    CustomerName = "Ricardo Arjona",
                    Location = "Coyoacán",
                    Status = "Pendiente",
                    DriverName = null,
                    DriverAvatar = null,
                    VehicleType = null,
                    EstimatedTime = null
                },
                new OrderItem
                {
                    Id = 2,
                    OrderNumber = "#4518",
                    ProductName = "Pastillas de Freno Cerámicas",
                    CustomerName = "Elena P.",
                    Location = "Polanco",
                    Status = "En Ruta",
                    DriverName = "Carlos Méndez",
                    DriverAvatar = "https://lh3.googleusercontent.com/aida-public/AB6AXuB8qLUIIjXlQavSQf7rywhzhe2K1VHRq36s52iOMd2cnvXrpALN5RIGO0BqiH4rapKD0_vaYma2FEo_vttgemBfYGG2LAcjphnKHzfH3wjUlkgUPmW2u1sh9YIg1miKyJJBh6j8zqZagtJRaMc6c3j8CGyGQbQrgeFIDUC24NkvI-A1tM7wcUC4w7T0hXH_hGfGOkZ-32OgKn06NJdEH-u_OfvyL1CnyS8Ortk_FNzzCTfUXn82-ab_JJJzENZTU4thwXhhrs7xTko",
                    VehicleType = "Motocicleta",
                    EstimatedTime = "12 min"
                },
                new OrderItem
                {
                    Id = 3,
                    OrderNumber = "#4509",
                    ProductName = "Amortiguadores (2)",
                    CustomerName = "Juan Valdés",
                    Location = "Satélite",
                    Status = "Entregado",
                    DriverName = null,
                    DriverAvatar = null,
                    VehicleType = null,
                    EstimatedTime = null
                },
                new OrderItem
                {
                    Id = 4,
                    OrderNumber = "#4522",
                    ProductName = "Filtro de Aceite y Aire",
                    CustomerName = "María González",
                    Location = "Roma Norte",
                    Status = "Pendiente",
                    DriverName = null,
                    DriverAvatar = null,
                    VehicleType = null,
                    EstimatedTime = null
                },
                new OrderItem
                {
                    Id = 5,
                    OrderNumber = "#4519",
                    ProductName = "Batería AGM 60Ah",
                    CustomerName = "Roberto Silva",
                    Location = "Condesa",
                    Status = "En Ruta",
                    DriverName = "Ana Martínez",
                    DriverAvatar = "https://lh3.googleusercontent.com/aida-public/AB6AXuAWAiIurANYCgBEWGYfjdTkrAWrxOyRnBXCFq4oZveKTFQBB_URTC1cge65dJjOgjqrjZHt15_TwaTnwCUv412EAFMTazJesjdJF1vCw5gTMbG7mt56shDTTxaf-5vF5A_whNYMjq8rUKmJ26TnEZN3p2MupDjyjITXspdi_ZYMijQGCg5gBr4FRncYYxAJYCxHBygvCIqwQqYkf2cOqglOX8rVOB1Z_gDl_MH1hsO7qqJPIBnktskdSoGL7XgqLjEoZ0H6t08yz0Q",
                    VehicleType = "Automóvil",
                    EstimatedTime = "18 min"
                },
                new OrderItem
                {
                    Id = 6,
                    OrderNumber = "#4515",
                    ProductName = "Neumáticos 195/65R15 (4)",
                    CustomerName = "Taller El Rayo",
                    Location = "Naucalpan",
                    Status = "Entregado",
                    DriverName = null,
                    DriverAvatar = null,
                    VehicleType = null,
                    EstimatedTime = null
                }
            };

            totalOrders = allOrders.Count;
            filteredOrders = new List<OrderItem>(allOrders);
        }

        private void LoadAvailableDrivers()
        {
            availableDrivers = new List<DriverOption>
            {
                new DriverOption
                {
                    Id = 1,
                    Name = "Marcos Ruiz",
                    Distance = "2.4 km",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuA9O2asDdOWZj03ma3h7kfBfMOgN5nzG_4X1YRGwU3CC0DijDZczBjaKCDeYwxitCHdCkHjDirdNNW9q0xjNUzQiC3FKlOXMHNzSFQGVHVjae9hp-xiW1b1MTWvOATy9JKmkxYyM3Axh0TkzLmasfU3ZFnWN_QYnZ5OPabZpvXjfrq7Z0KZT3BkoQ75sPG0SAAf3lYhCDK2qYOZ25OVM2LzYnoF03F2Kg8qP02AO8afO4LMgkB-3CfFgCXfL6_Z7bU-QNLPulY8Y6c",
                    IsSelected = false
                },
                new DriverOption
                {
                    Id = 2,
                    Name = "Pedro Sánchez",
                    Distance = "3.8 km",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuD_lVfTs_ZRoQqMl0xyZoWR4S5VKjZoB3xXQ75WUpr9B35fSOYEMy7WR4ab-AjYAibTx1Kib5gHtPeujyzWCjd1amxzwEeS2MUGuPyvIcJDDEb0IS9gz-20pbmx_b-QGGAE7zPBusJrf1j98OoS1OpRv3bM33_oOHCnWwmC4mCNBLrJeO7WNDEmb8uSeLqoeYpPY6bJKkao7Nn1JWra2ZzKtnEmFsMms3nFN5QR8f9Evls9ahUIo6hwTUCoPTU1__BJPToHfctx018",
                    IsSelected = false
                },
                new DriverOption
                {
                    Id = 3,
                    Name = "Laura Méndez",
                    Distance = "5.1 km",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAWAiIurANYCgBEWGYfjdTkrAWrxOyRnBXCFq4oZveKTFQBB_URTC1cge65dJjOgjqrjZHt15_TwaTnwCUv412EAFMTazJesjdJF1vCw5gTMbG7mt56shDTTxaf-5vF5A_whNYMjq8rUKmJ26TnEZN3p2MupDjyjITXspdi_ZYMijQGCg5gBr4FRncYYxAJYCxHBygvCIqwQqYkf2cOqglOX8rVOB1Z_gDl_MH1hsO7qqJPIBnktskdSoGL7XgqLjEoZ0H6t08yz0Q",
                    IsSelected = false
                }
            };
        }

        // Event Handlers
        private void ToggleMenu()
        {
            Console.WriteLine("🍔 Toggle menu");
        }

        private void ToggleSearch()
        {
            showSearch = !showSearch;
            StateHasChanged();
        }

        private void ToggleNotifications()
        {
            hasNotifications = false;
            StateHasChanged();
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

        private void ApplyFiltersAndSearch()
        {
            var result = allOrders.AsEnumerable();

            // Filtrar por estado
            if (selectedFilter != "Todos")
            {
                result = result.Where(o => o.Status.Equals(selectedFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Filtrar por búsqueda
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                result = result.Where(o =>
                    o.OrderNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    o.ProductName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    o.CustomerName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    o.Location.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                );
            }

            filteredOrders = result.ToList();
            StateHasChanged();
        }

        private void ClearFilters()
        {
            searchQuery = string.Empty;
            selectedFilter = "Todos";
            ApplyFiltersAndSearch();
        }

        // UI Helpers
        private string GetFilterChipClass(string filterValue)
        {
            var baseClass = "flex h-8 shrink-0 items-center justify-center gap-x-2 rounded-full px-5 transition-colors cursor-pointer text-xs font-bold lg:text-sm";
            if (selectedFilter == filterValue)
            {
                return $"{baseClass} bg-primary text-white";
            }
            return $"{baseClass} bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-400 hover:border-primary/50 border border-transparent";
        }

        private string GetOrderCardClass(string status)
        {
            var baseClass = "bg-gradient-to-br from-primary/10 to-primary/5 dark:from-primary/20 dark:to-primary/10 dark:bg-slate-900/50 rounded-xl p-4 border shadow-sm hover:shadow-lg transition-all cursor-pointer";

            if (status == "En Ruta")
            {
                return $"{baseClass} border-primary/20 ring-1 ring-primary/20";
            }

            return $"{baseClass} border-slate-200 dark:border-slate-800";
        }

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Pendiente" => "px-2 py-1 rounded bg-amber-500/10 border border-amber-500/20 text-amber-500 text-[10px] font-bold uppercase shrink-0",
                "En Ruta" => "px-2 py-1 rounded bg-primary/10 border border-primary/20 text-primary text-[10px] font-bold uppercase shrink-0",
                "Entregado" => "px-2 py-1 rounded bg-green-500/10 border border-green-500/20 text-green-500 text-[10px] font-bold uppercase shrink-0",
                _ => "px-2 py-1 rounded bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-400 text-[10px] font-bold uppercase shrink-0"
            };
        }

        private string GetActionButtonsGridClass(string status)
        {
            return status == "Pendiente" ? "grid grid-cols-3 gap-2" : "grid grid-cols-2 gap-2";
        }

        // Action Handlers
        private void ViewFullMap()
        {
            Console.WriteLine("🗺️ Ver mapa completo");
            // NavigationManager?.NavigateTo("/admin/orders/map");
        }

        private void ViewOrderDetails(int orderId)
        {
            Console.WriteLine($"👁️ Ver detalles del pedido: {orderId}");
            // NavigationManager?.NavigateTo($"/admin/orders/{orderId}");
        }

        private void AssignDriver(int orderId)
        {
            selectedOrderId = orderId;
            showAssignDriverModal = true;
            StateHasChanged();
            Console.WriteLine($"👤 Asignar repartidor al pedido: {orderId}");
        }

        private void ShowOrderMenu(int orderId)
        {
            Console.WriteLine($"⋮ Mostrar menú del pedido: {orderId}");
        }

        private void TrackOrder(int orderId)
        {
            Console.WriteLine($"📍 Rastrear pedido: {orderId}");
            // NavigationManager?.NavigateTo($"/admin/orders/{orderId}/track");
        }

        // Modal Actions
        private void CloseAssignModal()
        {
            showAssignDriverModal = false;
            selectedOrderId = 0;
            // Resetear selecciones
            foreach (var driver in availableDrivers)
            {
                driver.IsSelected = false;
            }
            StateHasChanged();
        }

        private void SelectDriver(int driverId)
        {
            // Deseleccionar todos
            foreach (var driver in availableDrivers)
            {
                driver.IsSelected = driver.Id == driverId;
            }
            StateHasChanged();
        }

        private void ConfirmAssignment(int driverId)
        {
            var driver = availableDrivers.FirstOrDefault(d => d.Id == driverId);
            var order = allOrders.FirstOrDefault(o => o.Id == selectedOrderId);

            if (driver != null && order != null)
            {
                Console.WriteLine($"✅ Asignar {driver.Name} al pedido {order.OrderNumber}");

                // Actualizar el pedido
                order.Status = "En Ruta";
                order.DriverName = driver.Name;
                order.DriverAvatar = driver.AvatarUrl;
                order.VehicleType = "Motocicleta"; // Esto vendría de los datos del driver
                order.EstimatedTime = "15 min";

                CloseAssignModal();
                ApplyFiltersAndSearch();
            }
        }

        // Data Models
        private class FilterOption
        {
            public string Label { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }

        private class OrderItem
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string? DriverName { get; set; }
            public string? DriverAvatar { get; set; }
            public string? VehicleType { get; set; }
            public string? EstimatedTime { get; set; }
        }

        private class DriverOption
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Distance { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            public bool IsSelected { get; set; }
        }
    }
}
