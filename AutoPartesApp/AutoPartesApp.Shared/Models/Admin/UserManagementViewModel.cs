using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Domain.Enums;
using AutoPartesApp.Shared.Services.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Models.Admin
{
    public class UserManagementViewModel
    {
        private readonly UserManagementService _userService;

        // Estado de carga
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        // Datos principales
        public List<UserListItemDto> Users { get; set; } = new();
        public UserStatsDto? Stats { get; set; }

        // Paginación
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        // Filtros
        public string SearchQuery { get; set; } = string.Empty;
        public RoleType? SelectedRole { get; set; }
        public bool? IsActiveFilter { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }

        // Tab seleccionado (para UI)
        public string SelectedTab { get; set; } = "all"; // "all", "clients", "deliveries"

        // Usuario seleccionado
        public UserDetailDto? SelectedUser { get; set; }

        public UserManagementViewModel(UserManagementService userService)
        {
            _userService = userService;
        }

        // ========================================
        // MÉTODOS DE CARGA
        // ========================================

        /// <summary>
        /// Cargar usuarios con filtros actuales
        /// </summary>
        public async Task LoadUsersAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var filters = new UserFilterDto
                {
                    SearchQuery = SearchQuery,
                    RoleType = SelectedRole,
                    IsActive = IsActiveFilter,
                    CreatedFrom = CreatedFrom,
                    CreatedTo = CreatedTo,
                    PageNumber = CurrentPage,
                    PageSize = PageSize
                };

                var result = await _userService.GetAllAsync(filters);

                if (result != null)
                {
                    Users = result.Items;
                    TotalCount = result.TotalCount;
                    TotalPages = result.TotalPages;
                    CurrentPage = result.PageNumber;
                }
                else
                {
                    ErrorMessage = "Error al cargar usuarios";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Cargar usuarios por tab (all, clients, deliveries)
        /// </summary>
        public async Task LoadUsersByTabAsync(string tab)
        {
            SelectedTab = tab;
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                List<UserListItemDto>? result = null;

                switch (tab)
                {
                    case "clients":
                        result = await _userService.GetCustomersAsync();
                        break;
                    case "deliveries":
                        result = await _userService.GetDeliveriesAsync();
                        break;
                    default:
                        await LoadUsersAsync();
                        return;
                }

                if (result != null)
                {
                    Users = result;
                    TotalCount = result.Count;
                    TotalPages = 1;
                }
                else
                {
                    ErrorMessage = "Error al cargar usuarios";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Cargar estadísticas
        /// </summary>
        public async Task LoadStatsAsync()
        {
            try
            {
                Stats = await _userService.GetStatsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar estadísticas: {ex.Message}");
            }
        }

        /// <summary>
        /// Cargar detalle de usuario
        /// </summary>
        public async Task LoadUserDetailAsync(string userId)
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                SelectedUser = await _userService.GetByIdAsync(userId);

                if (SelectedUser == null)
                {
                    ErrorMessage = "Usuario no encontrado";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ========================================
        // BÚSQUEDA Y FILTROS
        // ========================================

        /// <summary>
        /// Buscar usuarios
        /// </summary>
        public async Task SearchUsersAsync(string query)
        {
            SearchQuery = query;
            CurrentPage = 1; // Resetear a página 1
            await LoadUsersAsync();
        }

        /// <summary>
        /// Aplicar filtros
        /// </summary>
        public async Task ApplyFiltersAsync(RoleType? role = null, bool? isActive = null)
        {
            SelectedRole = role;
            IsActiveFilter = isActive;
            CurrentPage = 1;
            await LoadUsersAsync();
        }

        /// <summary>
        /// Limpiar filtros
        /// </summary>
        public async Task ClearFiltersAsync()
        {
            SearchQuery = string.Empty;
            SelectedRole = null;
            IsActiveFilter = null;
            CreatedFrom = null;
            CreatedTo = null;
            CurrentPage = 1;
            await LoadUsersAsync();
        }

        // ========================================
        // PAGINACIÓN
        // ========================================

        public async Task GoToPageAsync(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;
            await LoadUsersAsync();
        }

        public async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadUsersAsync();
            }
        }

        public async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadUsersAsync();
            }
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // ========================================
        // ACCIONES SOBRE USUARIOS
        // ========================================

        /// <summary>
        /// Bloquear/Desbloquear usuario
        /// </summary>
        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            try
            {
                var (success, error) = await _userService.ToggleStatusAsync(userId);

                if (success)
                {
                    SuccessMessage = "Estado del usuario actualizado correctamente";
                    await LoadUsersAsync(); // Recargar lista
                    return true;
                }
                else
                {
                    ErrorMessage = error ?? "Error al cambiar estado";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Cambiar rol del usuario
        /// </summary>
        public async Task<bool> ChangeUserRoleAsync(string userId, RoleType newRole, string? reason = null)
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            try
            {
                var dto = new ChangeRoleDto
                {
                    NewRole = newRole,
                    Reason = reason
                };

                var (success, error) = await _userService.ChangeRoleAsync(userId, dto);

                if (success)
                {
                    SuccessMessage = "Rol actualizado correctamente";
                    await LoadUsersAsync();
                    return true;
                }
                else
                {
                    ErrorMessage = error ?? "Error al cambiar rol";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Restablecer contraseña
        /// </summary>
        public async Task<string?> ResetPasswordAsync(string userId)
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            try
            {
                var (success, temporaryPassword, error) = await _userService.ResetPasswordAsync(userId);

                if (success)
                {
                    SuccessMessage = "Contraseña restablecida correctamente";
                    return temporaryPassword;
                }
                else
                {
                    ErrorMessage = error ?? "Error al restablecer contraseña";
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Eliminar usuario
        /// </summary>
        public async Task<bool> DeleteUserAsync(string userId)
        {
            IsLoading = true;
            ErrorMessage = null;
            SuccessMessage = null;

            try
            {
                var (success, error) = await _userService.DeleteAsync(userId);

                if (success)
                {
                    SuccessMessage = "Usuario eliminado correctamente";
                    await LoadUsersAsync();
                    return true;
                }
                else
                {
                    ErrorMessage = error ?? "Error al eliminar usuario";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ========================================
        // HELPERS
        // ========================================

        /// <summary>
        /// Obtener usuarios filtrados por búsqueda local (para UI)
        /// </summary>
        public List<UserListItemDto> GetFilteredUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return Users;

            var query = SearchQuery.ToLower();
            return Users.Where(u =>
                u.FullName.ToLower().Contains(query) ||
                u.Email.ToLower().Contains(query)
            ).ToList();
        }

        /// <summary>
        /// Obtener rango de páginas para paginador
        /// </summary>
        public List<int> GetPageRange(int maxPages = 5)
        {
            var halfRange = maxPages / 2;
            var startPage = Math.Max(1, CurrentPage - halfRange);
            var endPage = Math.Min(TotalPages, startPage + maxPages - 1);

            // Ajustar startPage si estamos cerca del final
            if (endPage - startPage < maxPages - 1)
            {
                startPage = Math.Max(1, endPage - maxPages + 1);
            }

            return Enumerable.Range(startPage, endPage - startPage + 1).ToList();
        }
    }
}
