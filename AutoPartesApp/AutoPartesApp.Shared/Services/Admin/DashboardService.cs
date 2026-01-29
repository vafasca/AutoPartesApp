using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class DashboardService
    {
        private readonly HttpClient _httpClient;

        public DashboardService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AutoPartesAPI");
        }

        public async Task<AdminDashboardDto?> GetAdminDashboardAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Dashboard/admin");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error en Dashboard API: {response.StatusCode}");
                    Console.WriteLine($"Detalle: {errorContent}");
                    return null;
                }

                var dashboardData = await response.Content.ReadFromJsonAsync<AdminDashboardDto>();
                Console.WriteLine($"✅ Dashboard data obtenida: {dashboardData?.Stats.TotalOrders} orders");
                return dashboardData;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ Error de conexión en DashboardService: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception en DashboardService: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}