using System.Net;
using System.Net.Http.Json;
using Orders_TW.Tests.Infrastructure;

namespace Orders_TW.Tests;

public class AuthEndpointsTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "admin"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<LoginResponsePayload>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
        Assert.Equal("admin", payload.Username);
        Assert.True(payload.ExpiresIn > 0);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "wrong-password"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
