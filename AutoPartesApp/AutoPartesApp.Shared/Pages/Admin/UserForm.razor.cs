using AutoPartesApp.Core.Application.DTOs.UserDTOs;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    partial class UserForm : ComponentBase
    {
        [Parameter]
        public string? UserId { get; set; }

        [Inject]
        private UserManagementService UserService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        private bool isLoading = false;
        private bool isSaving = false;
        private string? successMessage;
        private string? errorMessage;

        // Modelos específicos para crear y editar
        private CreateUserDto? createModel;
        private UpdateUserDto? updateModel;

        private bool IsEditMode => !string.IsNullOrEmpty(UserId);

        protected override async Task OnInitializedAsync()
        {
            if (IsEditMode)
            {
                await LoadUserForEditAsync();
            }
            else
            {
                createModel = new CreateUserDto();
            }
        }

        private async Task LoadUserForEditAsync()
        {
            isLoading = true;

            try
            {
                var user = await UserService.GetByIdAsync(UserId!);

                if (user != null)
                {
                    updateModel = new UpdateUserDto
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        AvatarUrl = user.AvatarUrl,
                        AddressStreet = user.AddressStreet,
                        AddressCity = user.AddressCity,
                        AddressState = user.AddressState,
                        AddressCountry = user.AddressCountry,
                        AddressZipCode = user.AddressZipCode
                    };
                }
                else
                {
                    errorMessage = "Usuario no encontrado";
                    await Task.Delay(2000);
                    GoBack();
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

        private async Task HandleValidSubmit()
        {
            isSaving = true;
            errorMessage = null;
            successMessage = null;

            try
            {
                if (IsEditMode)
                {
                    await UpdateUserAsync();
                }
                else
                {
                    await CreateUserAsync();
                }
            }
            finally
            {
                isSaving = false;
                StateHasChanged();
            }
        }

        private async Task CreateUserAsync()
        {
            if (createModel == null) return;

            var (success, user, error) = await UserService.CreateAsync(createModel);

            if (success && user != null)
            {
                successMessage = "Usuario creado exitosamente";
                await Task.Delay(1500);
                NavigationManager.NavigateTo($"/admin/users/{user.Id}");
            }
            else
            {
                errorMessage = error ?? "Error al crear usuario";
            }
        }

        private async Task UpdateUserAsync()
        {
            if (updateModel == null) return;

            var (success, user, error) = await UserService.UpdateAsync(UserId!, updateModel);

            if (success && user != null)
            {
                successMessage = "Usuario actualizado exitosamente";
                await Task.Delay(1500);
                NavigationManager.NavigateTo($"/admin/users/{UserId}");
            }
            else
            {
                errorMessage = error ?? "Error al actualizar usuario";
            }
        }

        private void GoBack()
        {
            if (IsEditMode && !string.IsNullOrEmpty(UserId))
            {
                NavigationManager.NavigateTo($"/admin/users/{UserId}");
            }
            else
            {
                NavigationManager.NavigateTo("/admin/users");
            }
        }
    }
}
