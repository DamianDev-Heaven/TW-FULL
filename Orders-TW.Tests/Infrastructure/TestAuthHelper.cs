using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Orders_TW.Tests.Infrastructure;

public static class TestAuthHelper
{
    public static async Task AuthenticateAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "admin"
        });

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LoginResponsePayload>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.Token))
        {
            throw new InvalidOperationException("No se pudo obtener token JWT para pruebas");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.Token);
    }
}
