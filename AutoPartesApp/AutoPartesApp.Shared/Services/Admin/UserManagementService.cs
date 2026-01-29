using AutoPartesApp.Core.Application.DTOs.Common;
using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace AutoPartesApp.Shared.Services.Admin
{
    public class UserManagementService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/v1/admin/users";

        public UserManagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ========================================
        // CONSULTAS (GET)
        // ========================================

        /// <summary>
        /// Obtener usuarios paginados con filtros
        /// </summary>
        public async Task<PagedResultDto<UserListItemDto>?> GetAllAsync(UserFilterDto filters)
        {
            try
            {
                var queryParams = BuildQueryString(filters);
                var response = await _httpClient.GetFromJsonAsync<PagedResultDto<UserListItemDto>>($"{BaseUrl}?{queryParams}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtener usuario por ID con detalles completos
        /// </summary>
        public async Task<UserDetailDto?> GetByIdAsync(string userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserDetailDto>($"{BaseUrl}/{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario {userId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtener usuarios por rol específico
        /// </summary>
        public async Task<List<UserListItemDto>?> GetByRoleAsync(RoleType roleType)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<UserListItemDto>>($"{BaseUrl}/by-role/{(int)roleType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios por rol: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtener solo clientes
        /// </summary>
        public async Task<List<UserListItemDto>?> GetCustomersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<UserListItemDto>>($"{BaseUrl}/customers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener clientes: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtener solo repartidores
        /// </summary>
        public async Task<List<UserListItemDto>?> GetDeliveriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<UserListItemDto>>($"{BaseUrl}/deliveries");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener repartidores: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Buscar usuarios por nombre o email
        /// </summary>
        public async Task<List<UserListItemDto>?> SearchAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return new List<UserListItemDto>();

                return await _httpClient.GetFromJsonAsync<List<UserListItemDto>>($"{BaseUrl}/search?query={Uri.EscapeDataString(query)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar usuarios: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtener estadísticas de usuarios
        /// </summary>
        public async Task<UserStatsDto?> GetStatsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserStatsDto>($"{BaseUrl}/stats");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estadísticas: {ex.Message}");
                return null;
            }
        }

        // ========================================
        // COMANDOS (POST, PUT, PATCH, DELETE)
        // ========================================

        /// <summary>
        /// Crear nuevo usuario
        /// </summary>
        public async Task<(bool Success, UserDetailDto? User, string? ErrorMessage)> CreateAsync(CreateUserDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDetailDto>();
                    return (true, user, null);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, null, $"Error {response.StatusCode}: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, null, $"Error al crear usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualizar usuario existente
        /// </summary>
        public async Task<(bool Success, UserDetailDto? User, string? ErrorMessage)> UpdateAsync(string userId, UpdateUserDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{userId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDetailDto>();
                    return (true, user, null);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, null, $"Error {response.StatusCode}: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, null, $"Error al actualizar usuario: {ex.Message}");
            }
        }

        /// <summary>
        /// Bloquear/Desbloquear usuario (toggle status)
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> ToggleStatusAsync(string userId)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"{BaseUrl}/{userId}/toggle-status", null);

                if (response.IsSuccessStatusCode)
                    return (true, null);

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Error {response.StatusCode}: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al cambiar estado: {ex.Message}");
            }
        }

        /// <summary>
        /// Cambiar rol del usuario
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> ChangeRoleAsync(string userId, ChangeRoleDto dto)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"{BaseUrl}/{userId}/change-role", dto);

                if (response.IsSuccessStatusCode)
                    return (true, null);

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Error {response.StatusCode}: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al cambiar rol: {ex.Message}");
            }
        }

        /// <summary>
        /// Restablecer contraseña del usuario
        /// </summary>
        public async Task<(bool Success, string? TemporaryPassword, string? ErrorMessage)> ResetPasswordAsync(string userId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/{userId}/reset-password", null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResetPasswordResponse>();
                    return (true, result?.TemporaryPassword, null);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, null, $"Error {response.StatusCode}: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, null, $"Error al restablecer contraseña: {ex.Message}");
            }
        }

        /// <summary>
        /// Eliminar usuario (soft delete)
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(string userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{userId}");

                if (response.IsSuccessStatusCode)
                    return (true, null);

                var errorContent = await response.Content.ReadAsStringAsync();
                return (false, $"Error {response.StatusCode}: {errorContent}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar usuario: {ex.Message}");
            }
        }

        // ========================================
        // HELPERS
        // ========================================

        private string BuildQueryString(UserFilterDto filters)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(filters.SearchQuery))
                queryParams.Add($"searchQuery={Uri.EscapeDataString(filters.SearchQuery)}");

            if (filters.RoleType.HasValue)
                queryParams.Add($"roleType={(int)filters.RoleType.Value}");

            if (filters.IsActive.HasValue)
                queryParams.Add($"isActive={filters.IsActive.Value}");

            if (filters.CreatedFrom.HasValue)
                queryParams.Add($"createdFrom={filters.CreatedFrom.Value:yyyy-MM-dd}");

            if (filters.CreatedTo.HasValue)
                queryParams.Add($"createdTo={filters.CreatedTo.Value:yyyy-MM-dd}");

            queryParams.Add($"pageNumber={filters.PageNumber}");
            queryParams.Add($"pageSize={filters.PageSize}");

            return string.Join("&", queryParams);
        }

        // Clase auxiliar para deserializar respuesta de reset password
        private class ResetPasswordResponse
        {
            public string Message { get; set; } = string.Empty;
            public string TemporaryPassword { get; set; } = string.Empty;
        }
    }
}
