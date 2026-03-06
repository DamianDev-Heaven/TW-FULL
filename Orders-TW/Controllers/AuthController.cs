using Microsoft.AspNetCore.Mvc;
using Orders_TW.DTOs.Auth;
using Orders_TW.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Orders_TW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Inicia sesión y devuelve JWT",
            Description = "Valida credenciales del usuario y retorna un token JWT con tiempo de expiración para consumir endpoints protegidos.")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.AuthenticateAsync(request);

            if (response == null)
            {
                return Unauthorized(new { message = "Usuario o contrase�a incorrectos" });
            }

            return Ok(response);
        }
    }
}