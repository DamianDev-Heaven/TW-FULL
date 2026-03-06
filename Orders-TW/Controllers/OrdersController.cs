using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Orders_TW.DTOs.Orders;
using Orders_TW.Models;
using Orders_TW.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Orders_TW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista órdenes paginadas",
            Description = "Obtiene órdenes con sus detalles usando parámetros opcionales page y pageSize.")]
        [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedOrders = await _orderService.GetPagedAsync(page, pageSize);
            return Ok(pagedOrders);
        }

        [HttpGet("all")]
        [SwaggerOperation(
            Summary = "Lista todas las órdenes",
            Description = "Obtiene todas las órdenes con sus detalles sin aplicar paginación.")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllWithoutPagination()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtiene una orden por ID",
            Description = "Busca una orden específica por su ID y devuelve 404 si no existe.")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            
            if (order == null)
                return NotFound(new { message = $"Orden con Id {id} no encontrada" });

            return Ok(order);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Crea una orden",
            Description = "Crea una nueva orden validando stock disponible, calculando el total automáticamente y reduciendo el stock de los productos. Requiere autenticación JWT.")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
        {
            var order = await _orderService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}