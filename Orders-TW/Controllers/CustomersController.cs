using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Orders_TW.DTOs.Customers;
using Orders_TW.Models;
using Orders_TW.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Orders_TW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista clientes paginados",
            Description = "Obtiene clientes con paginación usando parámetros opcionales page y pageSize.")]
        [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CustomerDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedCustomers = await _customerService.GetPagedAsync(page, pageSize);
            return Ok(pagedCustomers);
        }

        [HttpGet("all")]
        [SwaggerOperation(
            Summary = "Lista todos los clientes",
            Description = "Obtiene todos los clientes sin aplicar paginación.")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllWithoutPagination()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtiene un cliente por ID",
            Description = "Busca un cliente específico por su ID y devuelve 404 si no existe.")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            
            if (customer == null)
                return NotFound(new { message = $"Cliente con Id {id} no encontrado" });

            return Ok(customer);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Crea un cliente",
            Description = "Crea un nuevo cliente. Requiere autenticación JWT.")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerDto dto)
        {
            var customer = await _customerService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
    }
}