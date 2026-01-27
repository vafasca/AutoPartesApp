using System;
using System.Threading.Tasks;
using AutoPartesApp.Application.DTOs.InventoryDTOs;
using AutoPartesApp.Application.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    [ApiController]
    [Route("api/v1/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class InventoryController : ControllerBase
    {
        private readonly GetInventoryListUseCase _getInventoryListUseCase;
        private readonly UpdateStockUseCase _updateStockUseCase;
        private readonly DeleteProductUseCase _deleteProductUseCase;

        public InventoryController(
            GetInventoryListUseCase getInventoryListUseCase,
            UpdateStockUseCase updateStockUseCase,
            DeleteProductUseCase deleteProductUseCase)
        {
            _getInventoryListUseCase = getInventoryListUseCase;
            _updateStockUseCase = updateStockUseCase;
            _deleteProductUseCase = deleteProductUseCase;
        }

        [HttpGet("list")]
        public async Task<ActionResult<InventoryListDto>> GetInventoryList()
        {
            try
            {
                var result = await _getInventoryListUseCase.ExecuteAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpPost("update-stock")]
        public async Task<ActionResult> UpdateStock([FromBody] UpdateStockDto updateStockDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _updateStockUseCase.ExecuteAsync(updateStockDto);
                
                if (result)
                {
                    return Ok(new { message = "Stock actualizado correctamente" });
                }
                
                return BadRequest(new { message = "No se pudo actualizar el stock" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _deleteProductUseCase.ExecuteAsync(id);
                
                if (result)
                {
                    return Ok(new { message = "Producto eliminado lógicamente correctamente" });
                }
                
                return NotFound(new { message = "Producto no encontrado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}