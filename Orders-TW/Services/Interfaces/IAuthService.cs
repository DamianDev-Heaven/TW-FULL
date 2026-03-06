using Orders_TW.DTOs.Auth;

namespace Orders_TW.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
        string GenerateJwtToken(string username);
    }
}
