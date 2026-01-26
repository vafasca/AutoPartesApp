using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Core.Application.Admin.Dashboard
{
    public class GetAdminDashboardUseCase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public GetAdminDashboardUseCase(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<AdminDashboardDto> Execute()
        {
            // Obtener datos de los últimos 30 días
            var startDate = DateTime.UtcNow.AddDays(-30);
            var allOrders = await _orderRepository.GetAllAsync();
            var recentOrders = allOrders.Where(o => o.CreatedAt >= startDate).ToList();

            // Calcular estadísticas
            var totalSales = recentOrders.Sum(o => o.Total.Amount);
            var completedOrders = recentOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Delivered);
            var pendingOrders = recentOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Pending);

            var averageTicket = recentOrders.Any() ? totalSales / recentOrders.Count : 0;

            // Productos con bajo stock
            var lowStockProducts = await _productRepository.GetLowStockAsync(10);

            // Pedidos recientes (últimos 5)
            var recentOrderDtos = allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new RecentOrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.User?.FullName ?? "Cliente",
                    Status = GetStatusString(o.Status),
                    Total = o.Total.Amount,
                    TimeAgo = GetTimeAgo(o.CreatedAt),
                    ItemsCount = o.Items?.Count ?? 0,
                    ImageUrl = o.Items?.FirstOrDefault()?.Product?.ImageUrl
                })
                .ToList();

            // Datos para gráfico (últimos 7 días)
            var salesChart = GenerateSalesChartData(recentOrders);

            return new AdminDashboardDto
            {
                Stats = new DashboardStatsDto
                {
                    TotalSales = totalSales,
                    SalesChangePercentage = 12.5m, // Calcular comparando con período anterior
                    AverageTicket = averageTicket,
                    TicketChangePercentage = -2.1m,
                    ConversionRate = 3.2m,
                    ConversionChangePercentage = 0.5m,
                    TotalOrders = recentOrders.Count,
                    PendingOrders = pendingOrders,
                    CompletedOrders = completedOrders,
                    LowStockProducts = lowStockProducts.Count
                },
                RecentOrders = recentOrderDtos,
                SalesChart = salesChart
            };
        }

        private string GetStatusString(Domain.Enums.OrderStatus status)
        {
            return status switch
            {
                Domain.Enums.OrderStatus.Pending => "PENDIENTE",
                Domain.Enums.OrderStatus.Processing => "PROCESANDO",
                Domain.Enums.OrderStatus.Shipped => "ENVIADO",
                Domain.Enums.OrderStatus.Delivered => "COMPLETADO",
                Domain.Enums.OrderStatus.Cancelled => "CANCELADO",
                _ => "DESCONOCIDO"
            };
        }

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;

            if (timeSpan.TotalMinutes < 60)
                return $"hace {(int)timeSpan.TotalMinutes}m";
            if (timeSpan.TotalHours < 24)
                return $"hace {(int)timeSpan.TotalHours}h";
            if (timeSpan.TotalDays < 7)
                return $"hace {(int)timeSpan.TotalDays}d";

            return date.ToString("dd/MM/yyyy");
        }

        private SalesChartDataDto GenerateSalesChartData(List<Domain.Entities.Order> orders)
        {
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.UtcNow.Date.AddDays(-6 + i))
                .ToList();

            var chartData = new SalesChartDataDto();

            foreach (var day in last7Days)
            {
                var dayOrders = orders.Where(o => o.CreatedAt.Date == day).ToList();
                var dayTotal = dayOrders.Sum(o => o.Total.Amount);

                chartData.Labels.Add(day.ToString("ddd").ToUpper());
                chartData.Values.Add(dayTotal);
            }

            return chartData;
        }
    }
}
