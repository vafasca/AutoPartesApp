using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Dashboard : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private DashboardService? DashboardService { get; set; }

        // State
        private bool hasNotifications = false; // Inicializar en false, se actualizará con datos reales
        private string selectedPeriod = "week";
        private bool isLoading = true;
        private bool hasError = false;
        private string errorMessage = string.Empty;

        // Data from API - TODO VIENE DE LA BD
        private AdminDashboardDto? dashboardData;
        private AdminProfile adminProfile = new();
        private List<KpiMetric> kpiMetrics = new();
        private List<string> chartDays = new();
        private List<RecentOrder> recentOrders = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadDashboardDataFromApi();
            // Cargar perfil de admin después de tener los datos
            await LoadAdminProfileFromApi();
        }

        private async Task LoadDashboardDataFromApi()
        {
            isLoading = true;
            hasError = false;
            errorMessage = string.Empty;
            StateHasChanged();

            try
            {
                if (DashboardService == null)
                {
                    throw new InvalidOperationException("DashboardService no está disponible");
                }

                // OBTENER DATOS REALES DE LA BD
                dashboardData = await DashboardService.GetAdminDashboardAsync();

                if (dashboardData != null)
                {
                    // Mapear datos de la API a los modelos locales
                    MapDashboardData();

                    // Actualizar notificaciones con datos reales
                    // Puedes agregar una propiedad en AdminDashboardDto para esto
                    // hasNotifications = dashboardData.HasUnreadNotifications;
                }
                else
                {
                    hasError = true;
                    errorMessage = "No se pudieron cargar los datos del dashboard.";
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error de red al cargar dashboard: {ex.Message}");
                hasError = true;
                errorMessage = "Error de conexión. Verifique su conexión a internet.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar dashboard: {ex.Message}");
                hasError = true;
                errorMessage = "Error al cargar el dashboard. Intente nuevamente.";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadAdminProfileFromApi()
        {
            try
            {
                if (DashboardService == null) return;

                // TODO: Implementar método en DashboardService para obtener perfil del admin
                // var profile = await DashboardService.GetAdminProfileAsync();

                // Por ahora usar datos del dashboard si están disponibles
                if (dashboardData != null)
                {
                    adminProfile = new AdminProfile
                    {
                        // Estos datos deberían venir de tu sistema de autenticación
                        Name = "Administrador", // TODO: Obtener del contexto de autenticación
                        AvatarUrl = GetDefaultAvatar() // TODO: Obtener de la BD
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al cargar perfil de admin: {ex.Message}");
                // Usar valores por defecto si falla
                adminProfile = new AdminProfile
                {
                    Name = "Administrador",
                    AvatarUrl = GetDefaultAvatar()
                };
            }
        }

        private void MapDashboardData()
        {
            if (dashboardData == null) return;

            try
            {
                // Mapear KPIs CON DATOS REALES DE LA BD
                kpiMetrics = new List<KpiMetric>
                {
                    new KpiMetric
                    {
                        Title = "Ventas Totales",
                        Value = dashboardData.Stats.TotalSales.ToString("C"),
                        Icon = "trending_up",
                        IconColor = "text-primary",
                        ChangeText = FormatChangePercentage(dashboardData.Stats.SalesChangePercentage, "hoy"),
                        ChangeColor = GetChangeColor(dashboardData.Stats.SalesChangePercentage),
                        ChangeBgColor = GetChangeBgColor(dashboardData.Stats.SalesChangePercentage)
                    },
                    new KpiMetric
                    {
                        Title = "Ticket Prom.",
                        Value = dashboardData.Stats.AverageTicket.ToString("C"),
                        Icon = "payments",
                        IconColor = "text-orange-400",
                        ChangeText = FormatChangePercentage(dashboardData.Stats.TicketChangePercentage, "sem."),
                        ChangeColor = GetChangeColor(dashboardData.Stats.TicketChangePercentage),
                        ChangeBgColor = GetChangeBgColor(dashboardData.Stats.TicketChangePercentage)
                    },
                    new KpiMetric
                    {
                        Title = "Tasa Conv.",
                        Value = $"{dashboardData.Stats.ConversionRate:F1}%",
                        Icon = "ads_click",
                        IconColor = "text-blue-400",
                        ChangeText = FormatChangePercentage(dashboardData.Stats.ConversionChangePercentage, "mes"),
                        ChangeColor = GetChangeColor(dashboardData.Stats.ConversionChangePercentage),
                        ChangeBgColor = GetChangeBgColor(dashboardData.Stats.ConversionChangePercentage)
                    }
                };

                // Mapear días del gráfico CON DATOS REALES
                chartDays = dashboardData.SalesChart?.Labels ?? new List<string>();

                // Validar que haya labels
                if (chartDays.Count == 0)
                {
                    Console.WriteLine("⚠️ No hay datos de gráfico disponibles");
                }

                // Mapear pedidos recientes CON DATOS REALES DE LA BD
                recentOrders = dashboardData.RecentOrders?.Select(dto => new RecentOrder
                {
                    Id = ExtractOrderId(dto.Id),
                    OrderNumber = dto.OrderNumber ?? "N/A",
                    CustomerName = dto.CustomerName ?? "Cliente desconocido",
                    TimeAgo = dto.TimeAgo ?? "Hace un momento",
                    Total = dto.Total,
                    Status = dto.Status ?? "PENDIENTE",
                    Icon = GetIconForStatus(dto.Status ?? "PENDIENTE")
                }).ToList() ?? new List<RecentOrder>();

                Console.WriteLine($"✅ Dashboard cargado: {kpiMetrics.Count} KPIs, {recentOrders.Count} pedidos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al mapear datos del dashboard: {ex.Message}");
                // Inicializar listas vacías para evitar errores en la UI
                kpiMetrics = new List<KpiMetric>();
                chartDays = new List<string>();
                recentOrders = new List<RecentOrder>();
            }
        }

        // Helpers para formateo
        private string FormatChangePercentage(decimal percentage, string period)
        {
            var sign = percentage >= 0 ? "+" : "";
            return $"{sign}{percentage:F1}% {period}";
        }

        private string GetChangeColor(decimal percentage)
        {
            return percentage >= 0 ? "text-[#0bda5b]" : "text-[#fa6238]";
        }

        private string GetChangeBgColor(decimal percentage)
        {
            return percentage >= 0 ? "bg-[#0bda5b]/10" : "bg-[#fa6238]/10";
        }

        private int ExtractOrderId(string orderId)
        {
            try
            {
                // Intentar extraer el ID numérico del string
                var parts = orderId.Split('-');
                if (parts.Length > 0 && int.TryParse(parts[^1], out int id))
                {
                    return id;
                }
                // Si falla, intentar parsear el string completo
                if (int.TryParse(orderId, out int directId))
                {
                    return directId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al extraer OrderId de '{orderId}': {ex.Message}");
            }
            return 0; // ID por defecto si falla
        }

        private string GetIconForStatus(string status)
        {
            return status.ToUpper() switch
            {
                "PENDIENTE" => "receipt_long",
                "PROCESANDO" => "sync",
                "ENVIADO" => "local_shipping",
                "ENTREGADO" => "task_alt",
                "COMPLETADO" => "task_alt",
                "CANCELADO" => "cancel",
                _ => "receipt_long"
            };
        }

        private string GetDefaultAvatar()
        {
            // Avatar por defecto - puedes cambiarlo por uno de tu proyecto
            return "https://ui-avatars.com/api/?name=Admin&background=137fec&color=fff&size=128";
        }

        // UI Helpers
        private string GetPeriodButtonClass(string period)
        {
            return selectedPeriod == period
                ? "px-3 py-1 text-xs font-bold rounded-md bg-slate-300 dark:bg-slate-700 shadow-sm transition-colors"
                : "px-3 py-1 text-xs font-bold text-slate-500 dark:text-slate-400 hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors";
        }

        private string GetStatusBadgeClass(string status)
        {
            return status.ToUpper() switch
            {
                "PENDIENTE" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-yellow-500/10 text-yellow-500",
                "PROCESANDO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-blue-500/10 text-blue-500",
                "ENVIADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-primary/10 text-primary",
                "ENTREGADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-[#0bda5b]/10 text-[#0bda5b]",
                "COMPLETADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-[#0bda5b]/10 text-[#0bda5b]",
                "CANCELADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-red-500/10 text-red-500",
                _ => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-slate-100 dark:bg-slate-800 text-slate-600"
            };
        }

        // Event Handlers
        private void ToggleNotifications()
        {
            // TODO: Implementar lógica real de notificaciones
            // Podría abrir un panel lateral con notificaciones de la BD
            hasNotifications = false;
            StateHasChanged();
        }

        private async Task ChangePeriod(string period)
        {
            if (selectedPeriod == period) return;

            selectedPeriod = period;
            Console.WriteLine($"📊 Cambiando período a: {period}");

            // TODO: Implementar en DashboardService un método que acepte el período
            // await LoadDashboardDataFromApi(period);

            // Por ahora solo recargar los datos
            await LoadDashboardDataFromApi();
        }

        // Navigation
        private void GoToOrders() => NavigationManager?.NavigateTo("/admin/orders");
        private void GoToInventory() => NavigationManager?.NavigateTo("/admin/inventory");
        private void GoToReports() => NavigationManager?.NavigateTo("/admin/reports");
        private void ViewAllOrders() => NavigationManager?.NavigateTo("/admin/orders");

        private void ViewOrderDetail(int orderId)
        {
            if (orderId > 0)
            {
                NavigationManager?.NavigateTo($"/admin/orders/{orderId}");
            }
            else
            {
                Console.WriteLine($"⚠️ ID de pedido inválido: {orderId}");
            }
        }

        private async Task RetryLoadData()
        {
            await LoadDashboardDataFromApi();
        }

        // Data Models (Modelos locales para la UI)
        private class AdminProfile
        {
            public string Name { get; set; } = string.Empty;
            public string AvatarUrl { get; set; } = string.Empty;
        }

        private class KpiMetric
        {
            public string Title { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string IconColor { get; set; } = string.Empty;
            public string ChangeText { get; set; } = string.Empty;
            public string ChangeColor { get; set; } = string.Empty;
            public string ChangeBgColor { get; set; } = string.Empty;
        }

        private class RecentOrder
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string TimeAgo { get; set; } = string.Empty;
            public decimal Total { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }
    }
}