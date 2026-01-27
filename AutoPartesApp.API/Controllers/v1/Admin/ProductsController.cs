using AutoPartesApp.Core.Application.DTOs.AdminDTOs;
using AutoPartesApp.Core.Application.DTOs.Common;
using AutoPartesApp.Core.Application.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly GetAllProductsUseCase _getAllProductsUseCase;
        private readonly GetProductByIdUseCase _getProductByIdUseCase;
        private readonly CreateProductUseCase _createProductUseCase;
        private readonly UpdateProductUseCase _updateProductUseCase;
        private readonly UpdateProductStockUseCase _updateProductStockUseCase;
        private readonly ToggleProductStatusUseCase _toggleProductStatusUseCase;
        private readonly SearchProductsUseCase _searchProductsUseCase;
        private readonly GetLowStockUseCase _getLowStockUseCase;

        public ProductsController(
            GetAllProductsUseCase getAllProductsUseCase,
            GetProductByIdUseCase getProductByIdUseCase,
            CreateProductUseCase createProductUseCase,
            UpdateProductUseCase updateProductUseCase,
            UpdateProductStockUseCase updateProductStockUseCase,
            ToggleProductStatusUseCase toggleProductStatusUseCase,
            SearchProductsUseCase searchProductsUseCase,
            GetLowStockUseCase getLowStockUseCase)
        {
            _getAllProductsUseCase = getAllProductsUseCase;
            _getProductByIdUseCase = getProductByIdUseCase;
            _createProductUseCase = createProductUseCase;
            _updateProductUseCase = updateProductUseCase;
            _updateProductStockUseCase = updateProductStockUseCase;
            _toggleProductStatusUseCase = toggleProductStatusUseCase;
            _searchProductsUseCase = searchProductsUseCase;
            _getLowStockUseCase = getLowStockUseCase;
        }

        /// <summary>
        /// Obtener productos paginados con filtros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<ProductListItemDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto filter)
        {
            try
            {
                var result = await _getAllProductsUseCase.Execute(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener productos", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProductById(string id)
        {
            try
            {
                var product = await _getProductByIdUseCase.Execute(id);

                if (product == null)
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el producto", error = ex.Message });
            }
        }

        /// <summary>
        /// Crear nuevo producto
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _createProductUseCase.Execute(dto);

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = product.Id },
                    product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear el producto", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar producto existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _updateProductUseCase.Execute(id, dto);

                if (product == null)
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el producto", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar stock de un producto
        /// </summary>
        [HttpPatch("{id}/stock")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateStock(string id, [FromBody] UpdateStockDto dto)
        {
            try
            {
                // Asegurar que el ID del DTO coincida con el de la ruta
                dto.ProductId = id;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var success = await _updateProductStockUseCase.Execute(dto);

                if (!success)
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });

                return Ok(new { message = "Stock actualizado correctamente", newStock = dto.NewStock });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el stock", error = ex.Message });
            }
        }

        /// <summary>
        /// Activar/Desactivar producto
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            try
            {
                var success = await _toggleProductStatusUseCase.Execute(id);

                if (!success)
                    return NotFound(new { message = $"Producto con ID {id} no encontrado" });

                return Ok(new { message = "Estado del producto actualizado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cambiar el estado del producto", error = ex.Message });
            }
        }

        /// <summary>
        /// Buscar productos
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ProductListItemDto[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "El parámetro 'query' es requerido" });

                var products = await _searchProductsUseCase.Execute(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al buscar productos", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener productos con stock bajo
        /// </summary>
        [HttpGet("low-stock")]
        [ProducesResponseType(typeof(ProductListItemDto[]), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            try
            {
                var products = await _getLowStockUseCase.Execute(threshold);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener productos con stock bajo", error = ex.Message });
            }
        }
    }
}
