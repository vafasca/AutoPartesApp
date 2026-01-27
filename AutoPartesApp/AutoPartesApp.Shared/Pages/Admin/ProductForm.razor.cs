using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    partial class ProductForm : ComponentBase
    {
        [Inject]
        private InventoryService? InventoryService { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // Estado
        private bool isSaving = false;
        private CreateProductDto createDto = new CreateProductDto
        {
            Currency = "USD",
            Stock = 0,
            Price = 0
        };

        private async Task SaveProduct()
        {
            if (isSaving) return;

            isSaving = true;
            StateHasChanged();

            try
            {
                if (InventoryService == null)
                {
                    Console.WriteLine("❌ InventoryService no está inyectado");
                    return;
                }

                var created = await InventoryService.CreateProductAsync(createDto);

                if (created != null)
                {
                    Console.WriteLine("✅ Producto creado correctamente");
                    NavigationManager?.NavigateTo($"/admin/inventory/{created.Id}");
                }
                else
                {
                    Console.WriteLine("❌ Error al crear producto");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar producto: {ex.Message}");
            }
            finally
            {
                isSaving = false;
                StateHasChanged();
            }
        }

        private void GoBack()
        {
            NavigationManager?.NavigateTo("/admin/inventory");
        }
    }
}
