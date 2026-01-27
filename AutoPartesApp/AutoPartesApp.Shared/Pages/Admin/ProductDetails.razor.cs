using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Shared.Services.Admin;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Admin
{
    partial class ProductDetails : ComponentBase
    {
        [Parameter]
        public string ProductId { get; set; } = string.Empty;

        [Inject]
        private InventoryService? InventoryService { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // Estado
        private bool isLoading = true;
        private bool isEditMode = false;
        private ProductDto? product;
        private UpdateProductDto editDto = new();
        private int quickStockUpdate = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadProduct();
        }

        private async Task LoadProduct()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                if (InventoryService == null)
                {
                    Console.WriteLine("❌ InventoryService no está inyectado");
                    return;
                }

                product = await InventoryService.GetProductByIdAsync(ProductId);

                if (product != null)
                {
                    quickStockUpdate = product.Stock;
                    InitializeEditDto();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar producto: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void InitializeEditDto()
        {
            if (product == null) return;

            editDto = new UpdateProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Currency = product.Currency,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                Brand = product.Brand,
                Model = product.Model,
                Year = product.Year,
                Compatibility = product.Compatibility,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive
            };
        }

        private void EnableEditMode()
        {
            isEditMode = true;
            InitializeEditDto();
            StateHasChanged();
        }

        private void CancelEdit()
        {
            isEditMode = false;
            InitializeEditDto();
            StateHasChanged();
        }

        private async Task SaveChanges()
        {
            try
            {
                if (InventoryService == null || product == null) return;

                var updated = await InventoryService.UpdateProductAsync(ProductId, editDto);

                if (updated != null)
                {
                    product = updated;
                    isEditMode = false;
                    Console.WriteLine("✅ Producto actualizado correctamente");
                }
                else
                {
                    Console.WriteLine("❌ Error al actualizar producto");
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar cambios: {ex.Message}");
            }
        }

        private async Task UpdateStockQuick()
        {
            try
            {
                if (InventoryService == null) return;

                var success = await InventoryService.UpdateStockAsync(ProductId, quickStockUpdate);

                if (success)
                {
                    await LoadProduct();
                    Console.WriteLine("✅ Stock actualizado correctamente");
                }
                else
                {
                    Console.WriteLine("❌ Error al actualizar stock");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar stock: {ex.Message}");
            }
        }

        private async Task ToggleStatus()
        {
            try
            {
                if (InventoryService == null) return;

                var success = await InventoryService.ToggleProductStatusAsync(ProductId);

                if (success)
                {
                    await LoadProduct();
                    Console.WriteLine("✅ Estado actualizado correctamente");
                }
                else
                {
                    Console.WriteLine("❌ Error al actualizar estado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cambiar estado: {ex.Message}");
            }
        }

        private void GoBack()
        {
            NavigationManager?.NavigateTo("/admin/inventory");
        }

        private string GetStockBadgeClass(string status, int stock)
        {
            if (status == "Agotado" || stock == 0)
            {
                return "text-red-500 bg-red-500/10";
            }
            else if (stock <= 10 || status == "Bajo Stock")
            {
                return "text-amber-500 bg-amber-500/10";
            }
            return "text-emerald-500 bg-emerald-500/10";
        }
    }
}
