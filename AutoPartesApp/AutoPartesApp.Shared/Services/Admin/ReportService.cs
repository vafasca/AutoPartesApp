using AutoPartesApp.Core.Application.DTOs.ReportDTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class ReportService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/v1/admin/reports";

        public ReportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtiene reporte completo de ventas
        /// </summary>
        public async Task<SalesReportDto?> GetSalesReportAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? categoryId = null,
            ReportPeriodType? period = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") },
                { "categoryId", categoryId },
                { "period", period?.ToString() }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/sales{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SalesReportDto>();
        }

        /// <summary>
        /// Obtiene ventas agrupadas por categoría
        /// </summary>
        public async Task<List<SalesByCategoryDto>?> GetSalesByCategoryAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/sales-by-category{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<SalesByCategoryDto>>();
        }

        /// <summary>
        /// Obtiene los N productos más vendidos
        /// </summary>
        public async Task<List<TopProductDto>?> GetTopProductsAsync(
            int top = 10,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "top", top.ToString() },
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/top-products{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TopProductDto>>();
        }

        /// <summary>
        /// Obtiene reporte completo de inventario
        /// </summary>
        public async Task<InventoryReportDto?> GetInventoryReportAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/inventory");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<InventoryReportDto>();
        }

        /// <summary>
        /// Obtiene productos con baja rotación
        /// </summary>
        public async Task<List<TopProductDto>?> GetLowRotationProductsAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int threshold = 5)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") },
                { "threshold", threshold.ToString() }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/low-rotation-products{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TopProductDto>>();
        }

        /// <summary>
        /// Obtiene reporte completo de clientes
        /// </summary>
        public async Task<CustomerReportDto?> GetCustomerReportAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/customers{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CustomerReportDto>();
        }

        /// <summary>
        /// Obtiene los N mejores clientes
        /// </summary>
        public async Task<List<TopCustomerDto>?> GetTopCustomersAsync(
            int top = 10,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "top", top.ToString() },
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/top-customers{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TopCustomerDto>>();
        }

        /// <summary>
        /// Obtiene reporte completo de entregas
        /// </summary>
        public async Task<DeliveryReportDto?> GetDeliveryReportAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/deliveries{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DeliveryReportDto>();
        }

        /// <summary>
        /// Obtiene los N mejores repartidores
        /// </summary>
        public async Task<List<TopDriverDto>?> GetTopDriversAsync(
            int top = 10,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "top", top.ToString() },
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/top-drivers{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TopDriverDto>>();
        }

        /// <summary>
        /// Obtiene reporte completo de órdenes
        /// </summary>
        public async Task<OrderReportDto?> GetOrderReportAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/orders{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrderReportDto>();
        }

        /// <summary>
        /// Obtiene reporte financiero completo
        /// </summary>
        public async Task<FinancialReportDto?> GetFinancialReportAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/financial{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FinancialReportDto>();
        }

        /// <summary>
        /// Obtiene ingresos agrupados por período
        /// </summary>
        public async Task<List<PeriodRevenueDto>?> GetRevenueByPeriodAsync(
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            ReportPeriodType period = ReportPeriodType.Month)
        {
            var query = BuildQueryString(new Dictionary<string, string?>
            {
                { "dateFrom", dateFrom?.ToString("yyyy-MM-dd") },
                { "dateTo", dateTo?.ToString("yyyy-MM-dd") },
                { "period", period.ToString() }
            });

            var response = await _httpClient.GetAsync($"{_baseUrl}/revenue-by-period{query}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PeriodRevenueDto>>();
        }

        // Helper para construir query strings
        private string BuildQueryString(Dictionary<string, string?> parameters)
        {
            var validParams = parameters
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!)}");

            return validParams.Any() ? "?" + string.Join("&", validParams) : string.Empty;
        }
    }
}
