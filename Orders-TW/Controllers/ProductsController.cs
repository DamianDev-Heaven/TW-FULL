using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders_TW.DTOs.Products;
using Orders_TW.Models;
using Orders_TW.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Orders_TW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista productos paginados",
            Description = "Obtiene productos con paginación usando parámetros opcionales page y pageSize.")]
        [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedProducts = await _productService.GetPagedAsync(page, pageSize);
            return Ok(pagedProducts);
        }

        [HttpGet("all")]
        [SwaggerOperation(
            Summary = "Lista todos los productos",
            Description = "Obtiene todos los productos sin aplicar paginación.")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllWithoutPagination()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtiene un producto por ID",
            Description = "Busca un producto específico por su ID y devuelve 404 si no existe.")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Producto con Id {ProductId} no encontrado", id);
                return NotFound(new { message = $"Producto con Id {id} no encontrado" });
            }

            return Ok(product);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Crea un producto",
            Description = "Crea un nuevo producto. Requiere autenticación JWT.")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Actualiza un producto",
            Description = "Actualiza los datos de un producto existente por ID. Requiere autenticación JWT.")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _productService.UpdateAsync(id, dto);
            
            if (product == null)
            {
                _logger.LogWarning("Intento de actualizar producto inexistente. Id: {ProductId}", id);
                return NotFound(new { message = $"Producto con Id {id} no encontrado" });
            }

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Elimina un producto",
            Description = "Elimina un producto por ID. Requiere autenticación JWT.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            
            if (!deleted)
            {
                _logger.LogWarning("Intento de eliminar producto inexistente. Id: {ProductId}", id);
                return NotFound(new { message = $"Producto con Id {id} no encontrado" });
            }

            return NoContent();
        }
    }
}