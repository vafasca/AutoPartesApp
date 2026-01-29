using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    partial class UserDetails : ComponentBase
    {
        [Parameter]
        public string UserId { get; set; } = string.Empty;

        [Inject]
        private UserManagementService UserService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private bool isLoading = false;
        private UserDetailDto? user;
        private string? successMessage;
        private string? errorMessage;

        protected override async Task OnInitializedAsync()
        {
            await LoadUserAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(UserId))
            {
                await LoadUserAsync();
            }
        }

        private async Task LoadUserAsync()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                user = await UserService.GetByIdAsync(UserId);

                if (user == null)
                {
                    errorMessage = "Usuario no encontrado";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al cargar usuario: {ex.Message}";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/admin/users");
        }

        private void EditUser()
        {
            NavigationManager.NavigateTo($"/admin/users/{UserId}/edit");
        }

        private async Task ToggleStatus(string userId)
        {
            if (user == null) return;

            var action = user.IsActive ? "desactivar" : "activar";
            if (!await ConfirmAction($"¿Estás seguro de {action} este usuario?"))
                return;

            isLoading = true;
            errorMessage = null;

            try
            {
                var (success, error) = await UserService.ToggleStatusAsync(userId);

                if (success)
                {
                    successMessage = $"Usuario {action}do correctamente";
                    await LoadUserAsync();
                    await HideMessageAfterDelay();
                }
                else
                {
                    errorMessage = error ?? "Error al cambiar estado";
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

        private async Task ResetPassword(string userId)
        {
            if (!await ConfirmAction("¿Estás seguro de restablecer la contraseña de este usuario?"))
                return;

            isLoading = true;
            errorMessage = null;

            try
            {
                var (success, temporaryPassword, error) = await UserService.ResetPasswordAsync(userId);

                if (success && !string.IsNullOrEmpty(temporaryPassword))
                {
                    successMessage = $"Contraseña restablecida. Nueva contraseña temporal: {temporaryPassword}";
                    await HideMessageAfterDelay(5000); // 5 segundos para copiar
                }
                else
                {
                    errorMessage = error ?? "Error al restablecer contraseña";
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

        // UI Helpers
        private string GetRoleBadgeClass(Domain.Enums.RoleType roleType)
        {
            return roleType switch
            {
                Domain.Enums.RoleType.Admin => "bg-purple-100 text-purple-700 dark:bg-purple-900/30 dark:text-purple-400 text-xs font-bold px-3 py-1 rounded-full uppercase",
                Domain.Enums.RoleType.Delivery => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 text-xs font-bold px-3 py-1 rounded-full uppercase",
                Domain.Enums.RoleType.Client => "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 text-xs font-bold px-3 py-1 rounded-full uppercase",
                _ => "bg-slate-100 text-slate-700 dark:bg-slate-700 dark:text-slate-300 text-xs font-bold px-3 py-1 rounded-full uppercase"
            };
        }

        private string GetStatusBadgeClass(bool isActive)
        {
            return isActive
                ? "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 text-xs font-bold px-3 py-1 rounded-full uppercase"
                : "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400 text-xs font-bold px-3 py-1 rounded-full uppercase";
        }

        private string GetOrderStatusBadge(string status)
        {
            return status switch
            {
                "Delivered" => "bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400 text-xs font-bold px-2 py-1 rounded",
                "Cancelled" => "bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400 text-xs font-bold px-2 py-1 rounded",
                "Processing" => "bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 text-xs font-bold px-2 py-1 rounded",
                "Pending" => "bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400 text-xs font-bold px-2 py-1 rounded",
                _ => "bg-slate-100 text-slate-700 dark:bg-slate-700 dark:text-slate-300 text-xs font-bold px-2 py-1 rounded"
            };
        }

        private async Task<bool> ConfirmAction(string message)
        {
            // TODO: Implementar modal de confirmación
            Console.WriteLine($"⚠️ Confirmación: {message}");
            return await Task.FromResult(true);
        }

        private async Task HideMessageAfterDelay(int milliseconds = 3000)
        {
            await Task.Delay(milliseconds);
            successMessage = null;
            errorMessage = null;
            StateHasChanged();
        }
    }
}
