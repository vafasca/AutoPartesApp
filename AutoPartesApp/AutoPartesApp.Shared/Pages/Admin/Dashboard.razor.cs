using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Dashboard : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private DashboardService? DashboardService { get; set; }

        // State
        private bool hasNotifications = true;
        private string selectedPeriod = "week";
        private bool isLoading = true;

        // Data from API
        private AdminDashboardDto? dashboardData;
        private AdminProfile adminProfile = new();
        private List<KpiMetric> kpiMetrics = new();
        private List<string> chartDays = new();
        private List<RecentOrder> recentOrders = new();

        protected override async Task OnInitializedAsync()
        {
            LoadAdminProfile();
            await LoadDashboardDataFromApi();
        }

        private async Task LoadDashboardDataFromApi()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                dashboardData = await DashboardService!.GetAdminDashboardAsync();

                if (dashboardData != null)
                {
                    // Mapear datos de la API a los modelos locales
                    MapDashboardData();
                }
                else
                {
                    // Fallback a datos mock si la API falla
                    LoadMockData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dashboard: {ex.Message}");
                LoadMockData();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void MapDashboardData()
        {
            if (dashboardData == null) return;

            // Mapear KPIs
            kpiMetrics = new List<KpiMetric>
            {
                new KpiMetric
                {
                    Title = "Ventas Totales",
                    Value = dashboardData.Stats.TotalSales.ToString("C"),
                    Icon = "trending_up",
                    IconColor = "text-primary",
                    ChangeText = $"{(dashboardData.Stats.SalesChangePercentage >= 0 ? "+" : "")}{dashboardData.Stats.SalesChangePercentage:F1}% hoy",
                    ChangeColor = dashboardData.Stats.SalesChangePercentage >= 0 ? "text-[#0bda5b]" : "text-[#fa6238]",
                    ChangeBgColor = dashboardData.Stats.SalesChangePercentage >= 0 ? "bg-[#0bda5b]/10" : "bg-[#fa6238]/10"
                },
                new KpiMetric
                {
                    Title = "Ticket Prom.",
                    Value = dashboardData.Stats.AverageTicket.ToString("C"),
                    Icon = "payments",
                    IconColor = "text-orange-400",
                    ChangeText = $"{(dashboardData.Stats.TicketChangePercentage >= 0 ? "+" : "")}{dashboardData.Stats.TicketChangePercentage:F1}% sem.",
                    ChangeColor = dashboardData.Stats.TicketChangePercentage >= 0 ? "text-[#0bda5b]" : "text-[#fa6238]",
                    ChangeBgColor = dashboardData.Stats.TicketChangePercentage >= 0 ? "bg-[#0bda5b]/10" : "bg-[#fa6238]/10"
                },
                new KpiMetric
                {
                    Title = "Tasa Conv.",
                    Value = $"{dashboardData.Stats.ConversionRate:F1}%",
                    Icon = "ads_click",
                    IconColor = "text-blue-400",
                    ChangeText = $"{(dashboardData.Stats.ConversionChangePercentage >= 0 ? "+" : "")}{dashboardData.Stats.ConversionChangePercentage:F1}% mes",
                    ChangeColor = dashboardData.Stats.ConversionChangePercentage >= 0 ? "text-[#0bda5b]" : "text-[#fa6238]",
                    ChangeBgColor = dashboardData.Stats.ConversionChangePercentage >= 0 ? "bg-[#0bda5b]/10" : "bg-[#fa6238]/10"
                }
            };

            // Mapear días del gráfico
            chartDays = dashboardData.SalesChart.Labels;

            // Mapear pedidos recientes
            recentOrders = dashboardData.RecentOrders.Select(dto => new RecentOrder
            {
                Id = int.Parse(dto.Id.Split('-').Last()), // Simplificado
                OrderNumber = dto.OrderNumber,
                CustomerName = dto.CustomerName,
                TimeAgo = dto.TimeAgo,
                Total = dto.Total,
                Status = dto.Status,
                Icon = GetIconForStatus(dto.Status)
            }).ToList();

            Console.WriteLine($"✅ Dashboard mapeado: {kpiMetrics.Count} KPIs, {recentOrders.Count} orders");
        }

        private string GetIconForStatus(string status)
        {
            return status switch
            {
                "PENDIENTE" => "receipt_long",
                "ENVIADO" => "local_shipping",
                "COMPLETADO" => "task_alt",
                _ => "receipt_long"
            };
        }

        private void LoadMockData()
        {
            // Tu implementación mock actual como fallback
            LoadKpiMetrics();
            LoadChartDays();
            LoadRecentOrders();
        }

        private void LoadAdminProfile()
        {
            adminProfile = new AdminProfile
            {
                Name = "Administrador",
                AvatarUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAirWVAqjNh64G5RMGvb9QzA0A63kJwopve3MT3L0TPNifh7qWDmsYXo84IzKVmUjiWFA5pWJfj1qD55jn4PfbS6N7X01gfkQHeTnkgwsCeVVEiCMzSDLMt546X_2xDN4mbwhFFHB7LIYpS5R21Fy1IUkCAY4hO7AKMG-RU9dzvZw4zO1CN_IOjTUt559wFigic_Q_qj4OCMrgPOCmM7LGe-3ixaom-ITXlOqA-jPMkEL3TJ5Xdfi6QWJymqCXKRm_sAEAf3LrbbFg"
            };
        }

        private void LoadKpiMetrics()
        {
            kpiMetrics = new List<KpiMetric>
            {
                new KpiMetric
                {
                    Title = "Ventas Totales",
                    Value = "$12,450.00",
                    Icon = "trending_up",
                    IconColor = "text-primary",
                    ChangeText = "+12.5% hoy",
                    ChangeColor = "text-[#0bda5b]",
                    ChangeBgColor = "bg-[#0bda5b]/10"
                },
                new KpiMetric
                {
                    Title = "Ticket Prom.",
                    Value = "$85.00",
                    Icon = "payments",
                    IconColor = "text-orange-400",
                    ChangeText = "-2.1% sem.",
                    ChangeColor = "text-[#fa6238]",
                    ChangeBgColor = "bg-[#fa6238]/10"
                },
                new KpiMetric
                {
                    Title = "Tasa Conv.",
                    Value = "3.2%",
                    Icon = "ads_click",
                    IconColor = "text-blue-400",
                    ChangeText = "+0.5% mes",
                    ChangeColor = "text-[#0bda5b]",
                    ChangeBgColor = "bg-[#0bda5b]/10"
                }
            };
        }

        private void LoadChartDays()
        {
            chartDays = new List<string> { "LUN", "MAR", "MIÉ", "JUE", "VIE", "SÁB", "HOY" };
        }

        private void LoadRecentOrders()
        {
            recentOrders = new List<RecentOrder>
            {
                new RecentOrder
                {
                    Id = 1,
                    OrderNumber = "#ORD-2891",
                    CustomerName = "Juan Pérez",
                    TimeAgo = "hace 10m",
                    Total = 245.50m,
                    Status = "PENDIENTE",
                    Icon = "receipt_long"
                },
                new RecentOrder
                {
                    Id = 2,
                    OrderNumber = "#ORD-2890",
                    CustomerName = "María G.",
                    TimeAgo = "hace 1h",
                    Total = 1120.00m,
                    Status = "ENVIADO",
                    Icon = "local_shipping"
                },
                new RecentOrder
                {
                    Id = 3,
                    OrderNumber = "#ORD-2889",
                    CustomerName = "Taller \"El Rayo\"",
                    TimeAgo = "hace 3h",
                    Total = 89.00m,
                    Status = "COMPLETADO",
                    Icon = "task_alt"
                }
            };
        }

        // UI Helpers
        private string GetPeriodButtonClass(string period)
        {
            return selectedPeriod == period
                ? "px-3 py-1 text-xs font-bold rounded-md bg-slate-300 dark:bg-slate-700 shadow-sm"
                : "px-3 py-1 text-xs font-bold text-slate-500 dark:text-slate-400";
        }

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "PENDIENTE" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-yellow-500/10 text-yellow-500",
                "ENVIADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-primary/10 text-primary",
                "COMPLETADO" => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-[#0bda5b]/10 text-[#0bda5b]",
                _ => "text-[10px] px-2 py-0.5 rounded-full font-bold bg-slate-100 dark:bg-slate-800 text-slate-600"
            };
        }

        // Event Handlers
        private void ToggleNotifications()
        {
            hasNotifications = false;
            StateHasChanged();
        }

        private void ChangePeriod(string period)
        {
            selectedPeriod = period;
            Console.WriteLine($"📊 Cambio de período: {period}");
            // Recargar datos con el nuevo período
            _ = LoadDashboardDataFromApi();
        }

        private void GoToOrders() => NavigationManager?.NavigateTo("/admin/orders");
        private void GoToInventory() => NavigationManager?.NavigateTo("/admin/inventory");
        private void GoToReports() => NavigationManager?.NavigateTo("/admin/reports");
        private void ViewAllOrders() => NavigationManager?.NavigateTo("/admin/orders");

        private void ViewOrderDetail(int orderId)
        {
            Console.WriteLine($"Ver detalle del pedido: {orderId}");
        }

        // Data Models (mantener los existentes)
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
