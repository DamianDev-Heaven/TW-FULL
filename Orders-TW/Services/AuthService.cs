using Microsoft.IdentityModel.Tokens;
using Orders_TW.DTOs.Auth;
using Orders_TW.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Orders_TW.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            var isValid = await ValidateCredentialsAsync(request.Username, request.Password);
            
            if (!isValid)
            {
                _logger.LogWarning("Intento de login fallido para usuario: {Username}", request.Username);
                return null;
            }

            var token = GenerateJwtToken(request.Username);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

            _logger.LogInformation("Login exitoso para usuario: {Username}", request.Username);

            return new LoginResponse
            {
                Token = token,
                ExpiresIn = expirationMinutes * 60,
                Username = request.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
        }

        public string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");
            var issuer = jwtSettings["Issuer"] ?? "OrdersAPI";
            var audience = jwtSettings["Audience"] ?? "OrdersAPIUsers";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            // DEMO: Validación simple para pruebas
            // PRODUCCIÓN: Implementar con Identity o repositorio de usuarios
            // Ejemplo:
            // var user = await _userRepository.GetByUsernameAsync(username);
            // return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            
            return Task.FromResult(username == "admin" && password == "admin");
        }
    }
}