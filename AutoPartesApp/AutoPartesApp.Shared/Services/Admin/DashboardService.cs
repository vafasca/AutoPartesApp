using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

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
                    Console.WriteLine($"Error en Dashboard API: {response.StatusCode}");
                    return null;
                }

                var dashboardData = await response.Content.ReadFromJsonAsync<AdminDashboardDto>();

                Console.WriteLine($"Dashboard data obtenida: {dashboardData?.Stats.TotalOrders} orders");

                return dashboardData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception en DashboardService: {ex.Message}");
                return null;
            }
        }
    }
}
