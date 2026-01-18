using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Users : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private bool isLoading = false;
        private bool showSearch = false;
        private bool isMobileView = true;
        private string searchQuery = string.Empty;
        private string selectedTab = "clients"; // "clients" or "drivers"

        // Data
        private List<ClientUser> allClients = new();
        private List<ClientUser> filteredClients = new();
        private List<DriverUser> allDrivers = new();
        private List<DriverUser> filteredDrivers = new();

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

            LoadClients();
            LoadDrivers();

            isLoading = false;
            StateHasChanged();
        }

        private void CheckViewport()
        {
            // En producción, usar JS Interop para detectar el viewport
            // Por ahora asumimos mobile por defecto
            isMobileView = true;
        }

        private void LoadClients()
        {
            allClients = new List<ClientUser>
            {
                new ClientUser
                {
                    Id = 1,
                    Name = "Carlos Mendoza",
                    Email = "carlos.m@email.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDgGpcz1j2Xd_zfVUX_v33lCmOUYlwsU3qjxtrwZfM2Hd6w0ho7V56vYVMoWbj_8QMeEMLX3P_U2YwpNxF93Tusna0ERKSwzrDlQytoADfP7rwcbQus3ye5b2cTAwWfJfBk4mek50djA_aTLTULni3gOp7TRcJ5-Vl3mjJbSvaHL6CyoH9dxRK3SK3VSALvGQRECDUCTlS1ayFAhM3UUtgBk9O7REZAo1rRJhXHF2LGUI6MmGnEuAPn-lKJ-9RoPxzT2-jQjXVxf08",
                    Tier = "Premium",
                    Frequency = "Alta",
                    OrderCount = 32,
                    LastPurchase = "Hoy, 10:45 AM"
                },
                new ClientUser
                {
                    Id = 2,
                    Name = "Elena Rodríguez",
                    Email = "elena.r@email.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuArwzRhcrt7NVY_1EMecU1lWQKcTYt-aMbaefdMG-72NYu0nWItYukKG4okDWkpTRhtII2dq9ixNE48LqhKo_y_0sA2g7ThQd__vwaCF99CHQPx3PcZh2tCrc62MoQdckBA_3GUFQJjnDpboXLR0KmIzyN5hm5hqsPtDRp655Wr9LpsrbV8-Rfb2IzpQBQxLCEbGvmAlPb5WL0ScSKtbWjAxEG8MXvGVRFPOQPpF5qxjHvY3EWoxQyusTbJ8aMuonxTGmQld_Q9Pgc",
                    Tier = "Regular",
                    Frequency = "Media",
                    OrderCount = 12,
                    LastPurchase = "Hace 4 días"
                },
                new ClientUser
                {
                    Id = 3,
                    Name = "Miguel Ángel Torres",
                    Email = "miguel.t@email.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDgGpcz1j2Xd_zfVUX_v33lCmOUYlwsU3qjxtrwZfM2Hd6w0ho7V56vYVMoWbj_8QMeEMLX3P_U2YwpNxF93Tusna0ERKSwzrDlQytoADfP7rwcbQus3ye5b2cTAwWfJfBk4mek50djA_aTLTULni3gOp7TRcJ5-Vl3mjJbSvaHL6CyoH9dxRK3SK3VSALvGQRECDUCTlS1ayFAhM3UUtgBk9O7REZAo1rRJhXHF2LGUI6MmGnEuAPn-lKJ-9RoPxzT2-jQjXVxf08",
                    Tier = "Premium",
                    Frequency = "Alta",
                    OrderCount = 45,
                    LastPurchase = "Ayer, 15:20"
                },
                new ClientUser
                {
                    Id = 4,
                    Name = "Ana Martínez",
                    Email = "ana.m@email.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuArwzRhcrt7NVY_1EMecU1lWQKcTYt-aMbaefdMG-72NYu0nWItYukKG4okDWkpTRhtII2dq9ixNE48LqhKo_y_0sA2g7ThQd__vwaCF99CHQPx3PcZh2tCrc62MoQdckBA_3GUFQJjnDpboXLR0KmIzyN5hm5hqsPtDRp655Wr9LpsrbV8-Rfb2IzpQBQxLCEbGvmAlPb5WL0ScSKtbWjAxEG8MXvGVRFPOQPpF5qxjHvY3EWoxQyusTbJ8aMuonxTGmQld_Q9Pgc",
                    Tier = "Regular",
                    Frequency = "Baja",
                    OrderCount = 5,
                    LastPurchase = "Hace 2 semanas"
                }
            };

            filteredClients = new List<ClientUser>(allClients);
        }

        private void LoadDrivers()
        {
            allDrivers = new List<DriverUser>
            {
                new DriverUser
                {
                    Id = 1,
                    Name = "Roberto Sánchez",
                    Email = "roberto.s@delivery.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuD_lVfTs_ZRoQqMl0xyZoWR4S5VKjZoB3xXQ75WUpr9B35fSOYEMy7WR4ab-AjYAibTx1Kib5gHtPeujyzWCjd1amxzwEeS2MUGuPyvIcJDDEb0IS9gz-20pbmx_b-QGGAE7zPBusJrf1j98OoS1OpRv3bM33_oOHCnWwmC4mCNBLrJeO7WNDEmb8uSeLqoeYpPY6bJKkao7Nn1JWra2ZzKtnEmFsMms3nFN5QR8f9Evls9ahUIo6hwTUCoPTU1__BJPToHfctx018",
                    VehicleInfo = "Moto Honda 150cc • ID #4552",
                    VehicleIcon = "directions_bike",
                    IsOnline = true,
                    ActiveOrders = 1,
                    LastConnection = "En línea"
                },
                new DriverUser
                {
                    Id = 2,
                    Name = "Lucía Gómez",
                    Email = "lucia.g@delivery.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAWAiIurANYCgBEWGYfjdTkrAWrxOyRnBXCFq4oZveKTFQBB_URTC1cge65dJjOgjqrjZHt15_TwaTnwCUv412EAFMTazJesjdJF1vCw5gTMbG7mt56shDTTxaf-5vF5A_whNYMjq8rUKmJ26TnEZN3p2MupDjyjITXspdi_ZYMijQGCg5gBr4FRncYYxAJYCxHBygvCIqwQqYkf2cOqglOX8rVOB1Z_gDl_MH1hsO7qqJPIBnktskdSoGL7XgqLjEoZ0H6t08yz0Q",
                    VehicleInfo = "Furgoneta Sprinter • ID #1029",
                    VehicleIcon = "local_shipping",
                    IsOnline = false,
                    ActiveOrders = 0,
                    LastConnection = "Hace 2 horas"
                },
                new DriverUser
                {
                    Id = 3,
                    Name = "Pedro Ramírez",
                    Email = "pedro.r@delivery.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuD_lVfTs_ZRoQqMl0xyZoWR4S5VKjZoB3xXQ75WUpr9B35fSOYEMy7WR4ab-AjYAibTx1Kib5gHtPeujyzWCjd1amxzwEeS2MUGuPyvIcJDDEb0IS9gz-20pbmx_b-QGGAE7zPBusJrf1j98OoS1OpRv3bM33_oOHCnWwmC4mCNBLrJeO7WNDEmb8uSeLqoeYpPY6bJKkao7Nn1JWra2ZzKtnEmFsMms3nFN5QR8f9Evls9ahUIo6hwTUCoPTU1__BJPToHfctx018",
                    VehicleInfo = "Moto Yamaha 125cc • ID #2201",
                    VehicleIcon = "directions_bike",
                    IsOnline = true,
                    ActiveOrders = 2,
                    LastConnection = "En línea"
                },
                new DriverUser
                {
                    Id = 4,
                    Name = "Carmen Silva",
                    Email = "carmen.s@delivery.com",
                    AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAWAiIurANYCgBEWGYfjdTkrAWrxOyRnBXCFq4oZveKTFQBB_URTC1cge65dJjOgjqrjZHt15_TwaTnwCUv412EAFMTazJesjdJF1vCw5gTMbG7mt56shDTTxaf-5vF5A_whNYMjq8rUKmJ26TnEZN3p2MupDjyjITXspdi_ZYMijQGCg5gBr4FRncYYxAJYCxHBygvCIqwQqYkf2cOqglOX8rVOB1Z_gDl_MH1hsO7qqJPIBnktskdSoGL7XgqLjEoZ0H6t08yz0Q",
                    VehicleInfo = "Auto Compacto • ID #8891",
                    VehicleIcon = "directions_car",
                    IsOnline = true,
                    ActiveOrders = 0,
                    LastConnection = "En línea"
                }
            };

            filteredDrivers = new List<DriverUser>(allDrivers);
        }

        // Event Handlers
        private void ToggleSearch()
        {
            showSearch = !showSearch;
            StateHasChanged();
        }

        private void ToggleFilters()
        {
            Console.WriteLine("🔍 Toggle filters");
        }

        private void HandleSearch()
        {
            ApplySearch();
        }

        private void ChangeTab(string tab)
        {
            selectedTab = tab;
            searchQuery = string.Empty;
            ApplySearch();
        }

        private void ApplySearch()
        {
            if (selectedTab == "clients")
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    filteredClients = new List<ClientUser>(allClients);
                }
                else
                {
                    filteredClients = allClients.Where(c =>
                        c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        c.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                {
                    filteredDrivers = new List<DriverUser>(allDrivers);
                }
                else
                {
                    filteredDrivers = allDrivers.Where(d =>
                        d.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        d.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
            }

            StateHasChanged();
        }

        // UI Helpers
        private string GetUserBorderClass(string tier)
        {
            return tier == "Premium" ? "border-primary" : "border-slate-300 dark:border-slate-600";
        }

        private string GetTierBadgeClass(string tier)
        {
            return tier switch
            {
                "Premium" => "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 text-[10px] font-bold px-2 py-0.5 rounded-full uppercase shrink-0",
                "Regular" => "bg-slate-100 text-slate-600 dark:bg-slate-700 dark:text-slate-300 text-[10px] font-bold px-2 py-0.5 rounded-full uppercase shrink-0",
                _ => "bg-slate-100 text-slate-600 dark:bg-slate-700 dark:text-slate-300 text-[10px] font-bold px-2 py-0.5 rounded-full uppercase shrink-0"
            };
        }

        private string GetFrequencyColor(string frequency)
        {
            return frequency switch
            {
                "Alta" => "text-primary",
                "Media" => "text-yellow-600 dark:text-yellow-500",
                "Baja" => "text-slate-500",
                _ => "text-slate-500"
            };
        }

        private string GetOnlineStatusClass(bool isOnline)
        {
            return isOnline
                ? "absolute bottom-0 right-0 w-4 h-4 bg-green-500 border-2 border-white dark:border-[#192633] rounded-full"
                : "absolute bottom-0 right-0 w-4 h-4 bg-slate-400 border-2 border-white dark:border-[#192633] rounded-full";
        }

        private string GetDriverStatusBadgeClass(bool isOnline)
        {
            return isOnline
                ? "text-green-500 text-[10px] font-bold uppercase shrink-0"
                : "text-slate-500 text-[10px] font-bold uppercase shrink-0";
        }

        // Action Handlers - Clients
        private void SendPromotion(int clientId)
        {
            var client = allClients.FirstOrDefault(c => c.Id == clientId);
            Console.WriteLine($"📧 Enviar promoción a: {client?.Name}");
            // Implementar lógica de envío de promoción
        }

        private void BlockUser(int userId, string userName)
        {
            Console.WriteLine($"🚫 Bloquear usuario: {userName}");
            // Implementar modal de confirmación y lógica de bloqueo
        }

        private void ExportClients()
        {
            Console.WriteLine("📥 Exportar lista de clientes");
            // Implementar exportación a CSV/Excel
        }

        // Action Handlers - Drivers
        private void AssignOrder(int driverId)
        {
            var driver = allDrivers.FirstOrDefault(d => d.Id == driverId);
            Console.WriteLine($"📦 Asignar pedido a: {driver?.Name}");
            // NavigationManager?.NavigateTo($"/admin/assign-order/{driverId}");
        }

        private void ViewDriverLocation(int driverId)
        {
            var driver = allDrivers.FirstOrDefault(d => d.Id == driverId);
            Console.WriteLine($"📍 Ver ubicación de: {driver?.Name}");
            // NavigationManager?.NavigateTo($"/admin/driver-location/{driverId}");
        }

        private void ViewDriversMap()
        {
            Console.WriteLine("🗺️ Ver todos los repartidores en el mapa");
            // NavigationManager?.NavigateTo("/admin/drivers-map");
        }

        // Data Models
        private class ClientUser
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            public string Tier { get; set; } = string.Empty;
            public string Frequency { get; set; } = string.Empty;
            public int OrderCount { get; set; }
            public string LastPurchase { get; set; } = string.Empty;
        }

        private class DriverUser
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
            public string VehicleInfo { get; set; } = string.Empty;
            public string VehicleIcon { get; set; } = string.Empty;
            public bool IsOnline { get; set; }
            public int ActiveOrders { get; set; }
            public string LastConnection { get; set; } = string.Empty;
        }
    }
}
