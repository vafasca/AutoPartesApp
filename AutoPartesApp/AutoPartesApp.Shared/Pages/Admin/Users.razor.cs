using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    public partial class Users : ComponentBase
    {
        [Inject]
        private UserManagementService UserService { get; set; } = default!;

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // ========================================
        // ESTADO DE LA UI
        // ========================================

        private bool isLoading = false;
        private bool showSearch = false;
        private bool showFilters = false;
        private bool isMobileView = true;
        private string searchQuery = string.Empty;
        private string selectedTab = "clients"; // "clients" or "drivers"

        // Mensajes
        private string? successMessage;
        private string? errorMessage;

        // ========================================
        // DATOS
        // ========================================

        private List<UserListItemDto> allUsers = new();
        private List<UserListItemDto> filteredUsers = new();
        private UserStatsDto? stats;

        // Filtros
        private RoleType? selectedRoleFilter = null;
        private bool? selectedStatusFilter = null;

        // ========================================
        // LIFECYCLE
        // ========================================

        protected override async Task OnInitializedAsync()
        {
            await LoadInitialDataAsync();
            CheckViewport();
        }

        private async Task LoadInitialDataAsync()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                // Cargar estadísticas
                stats = await UserService.GetStatsAsync();

                // Cargar usuarios según el tab seleccionado
                await LoadUsersByTabAsync(selectedTab);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al cargar datos: {ex.Message}";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void CheckViewport()
        {
            // En producción, usar JS Interop para detectar el viewport
            // Por ahora asumimos mobile por defecto
            isMobileView = true;
        }

        // ========================================
        // CARGA DE DATOS
        // ========================================

        private async Task LoadUsersByTabAsync(string tab)
        {
            selectedTab = tab;
            errorMessage = null;
            isLoading = true;

            try
            {
                List<UserListItemDto>? users = null;

                switch (tab)
                {
                    case "clients":
                        users = await UserService.GetCustomersAsync();
                        break;
                    case "drivers":
                        users = await UserService.GetDeliveriesAsync();
                        break;
                    default:
                        // Cargar todos
                        var filters = new UserFilterDto
                        {
                            PageNumber = 1,
                            PageSize = 100
                        };
                        var result = await UserService.GetAllAsync(filters);
                        users = result?.Items;
                        break;
                }

                if (users != null)
                {
                    allUsers = users;
                    ApplyLocalFilters();
                }
                else
                {
                    errorMessage = "Error al cargar usuarios";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void ApplyLocalFilters()
        {
            var query = allUsers.AsEnumerable();

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerQuery = searchQuery.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(lowerQuery) ||
                    u.Email.ToLower().Contains(lowerQuery));
            }

            // Filtro por estado
            if (selectedStatusFilter.HasValue)
            {
                query = query.Where(u => u.IsActive == selectedStatusFilter.Value);
            }

            filteredUsers = query.ToList();
        }

        // ========================================
        // EVENT HANDLERS - UI
        // ========================================

        private void ToggleSearch()
        {
            showSearch = !showSearch;
            StateHasChanged();
        }

        private void ToggleFilters()
        {
            showFilters = !showFilters;
            StateHasChanged();
        }

        private void HandleSearch()
        {
            ApplyLocalFilters();
            StateHasChanged();
        }

        private async Task ChangeTab(string tab)
        {
            if (selectedTab != tab)
            {
                searchQuery = string.Empty;
                selectedStatusFilter = null;
                await LoadUsersByTabAsync(tab);
            }
        }

        // ========================================
        // ACCIONES - CLIENTES
        // ========================================

        private async Task SendPromotion(string userId)
        {
            var user = allUsers.FirstOrDefault(u => u.Id == userId);
            Console.WriteLine($"📧 Enviar promoción a: {user?.FullName}");
            // TODO: Implementar lógica de envío de promoción
            successMessage = $"Promoción enviada a {user?.FullName}";
            await Task.Delay(2000);
            successMessage = null;
            StateHasChanged();
        }

        private async Task BlockUser(string userId, string userName)
        {
            if (!await ConfirmAction($"¿Estás seguro de bloquear a {userName}?"))
                return;

            isLoading = true;
            errorMessage = null;

            try
            {
                var (success, error) = await UserService.ToggleStatusAsync(userId);

                if (success)
                {
                    successMessage = $"Estado de {userName} actualizado correctamente";
                    await LoadUsersByTabAsync(selectedTab);
                }
                else
                {
                    errorMessage = error ?? "Error al bloquear usuario";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task ExportClients()
        {
            Console.WriteLine("📥 Exportar lista de clientes");
            // TODO: Implementar exportación a CSV/Excel
            successMessage = "Exportación iniciada...";
            await Task.Delay(2000);
            successMessage = null;
            StateHasChanged();
        }

        // ========================================
        // ACCIONES - REPARTIDORES
        // ========================================

        private void AssignOrder(string driverId)
        {
            var driver = allUsers.FirstOrDefault(d => d.Id == driverId);
            Console.WriteLine($"📦 Asignar pedido a: {driver?.FullName}");
            NavigationManager?.NavigateTo($"/admin/orders?assignTo={driverId}");
        }

        private void ViewDriverLocation(string driverId)
        {
            var driver = allUsers.FirstOrDefault(d => d.Id == driverId);
            Console.WriteLine($"📍 Ver ubicación de: {driver?.FullName}");
            NavigationManager?.NavigateTo($"/admin/tracking/{driverId}");
        }

        private void ViewDriversMap()
        {
            Console.WriteLine("🗺️ Ver todos los repartidores en el mapa");
            NavigationManager?.NavigateTo("/admin/deliveries/map");
        }

        // ========================================
        // NAVEGACIÓN
        // ========================================

        private void ViewUserDetails(string userId)
        {
            NavigationManager?.NavigateTo($"/admin/users/{userId}");
        }

        private void CreateNewUser()
        {
            NavigationManager?.NavigateTo("/admin/users/new");
        }

        private void EditUser(string userId)
        {
            NavigationManager?.NavigateTo($"/admin/users/{userId}/edit");
        }

        // ========================================
        // UI HELPERS
        // ========================================

        // ✅ CORRECTO
        private string GetUserBorderClass(int totalOrders)
        {
            return totalOrders > 20
                ? "border-primary"
                : "border-slate-300 dark:border-slate-600";
        }

        private string GetTierBadgeClass(int totalOrders)
        {
            var tier = totalOrders > 20 ? "Premium" : "Regular";

            return tier switch
            {
                "Premium" => "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 text-[10px] font-bold px-2 py-0.5 rounded-full uppercase shrink-0",
                "Regular" => "bg-slate-100 text-slate-600 dark:bg-slate-700 dark:text-slate-300 text-[10px] font-bold px-2 py-0.5 rounded-full uppercase shrink-0",
                _ => "bg-slate-100 text-slate-600 dark:bg-slate-700 dark:text-slate-300 text-[10px] font-bold px-2 py-0.5 rounded-full uppercase shrink-0"
            };
        }

        private string GetFrequencyColor(int totalOrders)
        {
            if (totalOrders > 20) return "text-primary";
            if (totalOrders > 10) return "text-yellow-600 dark:text-yellow-500";
            return "text-slate-500";
        }

        private string GetFrequencyLabel(int totalOrders)
        {
            if (totalOrders > 20) return "Alta";
            if (totalOrders > 10) return "Media";
            return "Baja";
        }

        private string GetOnlineStatusClass(bool isActive)
        {
            return isActive
                ? "absolute bottom-0 right-0 w-4 h-4 bg-green-500 border-2 border-white dark:border-[#192633] rounded-full"
                : "absolute bottom-0 right-0 w-4 h-4 bg-slate-400 border-2 border-white dark:border-[#192633] rounded-full";
        }

        private string GetDriverStatusBadgeClass(bool isActive)
        {
            return isActive
                ? "text-green-500 text-[10px] font-bold uppercase shrink-0"
                : "text-slate-500 text-[10px] font-bold uppercase shrink-0";
        }

        private string GetVehicleIcon(string userId)
        {
            // En producción, esto vendría de los datos del usuario
            return "directions_bike"; // Por defecto moto
        }

        private string GetVehicleInfo(string userId)
        {
            // En producción, esto vendría de los datos del usuario
            return "Moto Honda 150cc • ID #" + userId.Substring(0, 4);
        }

        private string GetLastConnection(UserListItemDto user)
        {
            if (user.LastLoginAt == null)
                return "Nunca";

            var diff = DateTime.UtcNow - user.LastLoginAt.Value;

            if (diff.TotalMinutes < 60)
                return $"Hace {(int)diff.TotalMinutes} min";
            if (diff.TotalHours < 24)
                return $"Hace {(int)diff.TotalHours} horas";
            if (diff.TotalDays < 30)
                return $"Hace {(int)diff.TotalDays} días";

            return user.LastLoginAt.Value.ToString("dd/MM/yyyy");
        }

        private string FormatLastPurchase(UserListItemDto user)
        {
            if (user.LastLoginAt == null)
                return "Sin compras";

            var diff = DateTime.UtcNow - user.LastLoginAt.Value;

            if (diff.TotalHours < 24)
                return "Hoy, " + user.LastLoginAt.Value.ToString("HH:mm");
            if (diff.TotalDays < 7)
                return $"Hace {(int)diff.TotalDays} días";

            return user.LastLoginAt.Value.ToString("dd/MM/yyyy");
        }

        // ========================================
        // HELPERS
        // ========================================

        private async Task<bool> ConfirmAction(string message)
        {
            // TODO: Implementar modal de confirmación
            // Por ahora retorna true
            Console.WriteLine($"⚠️ Confirmación: {message}");
            return await Task.FromResult(true);
        }

        private void ClearMessages()
        {
            successMessage = null;
            errorMessage = null;
        }
    }
}
