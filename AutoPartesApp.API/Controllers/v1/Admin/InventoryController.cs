using Microsoft.AspNetCore.Mvc;
using AutoPartesApp.Domain.Interfaces;
using AutoPartesApp.Core.Application.Inventory;
using AutoPartesApp.Domain.Entities;

namespace AutoPartesApp.API.Controllers.v1.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly CreateProductUseCase _createProductUseCase;
        private readonly UpdateProductUseCase _updateProductUseCase;
        private readonly GetLowStockUseCase _getLowStockUseCase;

        public InventoryController(
            IProductRepository productRepository,
            CreateProductUseCase createProductUseCase,
            UpdateProductUseCase updateProductUseCase,
            GetLowStockUseCase getLowStockUseCase)
        {
            _productRepository = productRepository;
            _createProductUseCase = createProductUseCase;
            _updateProductUseCase = updateProductUseCase;
            _getLowStockUseCase = getLowStockUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                var createdProduct = await _createProductUseCase.Execute(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("ID del producto no coincide");
            }

            var updatedProduct = await _updateProductUseCase.Execute(id, product);
            if (updatedProduct == null)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var result = await _productRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Producto con ID {id} no encontrado");
            }
            return Ok(new { message = "Producto eliminado exitosamente" });
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Product>>> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            var lowStockProducts = await _getLowStockUseCase.Execute(threshold);
            return Ok(lowStockProducts);
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetInventoryStats()
        {
            var allProducts = await _productRepository.GetAllAsync();
            var lowStockProducts = await _getLowStockUseCase.Execute(10);

            var stats = new
            {
                totalProducts = allProducts.Count,
                totalValue = allProducts.Sum(p => p.Price.Amount * p.Stock),
                lowStockCount = lowStockProducts.Count,
                outOfStockCount = allProducts.Count(p => p.Stock == 0)
            };

            return Ok(stats);
        }
    }
}