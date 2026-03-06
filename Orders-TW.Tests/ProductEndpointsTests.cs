using System.Net;
using System.Net.Http.Json;
using Orders_TW.Tests.Infrastructure;

namespace Orders_TW.Tests;

public class ProductEndpointsTests
{
    [Fact]
    public async Task GetProductsPaged_ReturnsOkWithPagination()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products?page=1&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PagedResultPayload<ProductPayload>>();
        Assert.NotNull(payload);
        Assert.Equal(1, payload.Page);
        Assert.Equal(5, payload.PageSize);
        Assert.True(payload.Items.Count <= 5);
        Assert.True(payload.TotalCount > 0);
    }

    [Fact]
    public async Task GetProductsAll_ReturnsOk()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products/all");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<ProductPayload>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
    }

    [Fact]
    public async Task GetProductById_WhenExists_ReturnsOk()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ProductPayload>();
        Assert.NotNull(payload);
        Assert.Equal(1, payload.Id);
    }

    [Fact]
    public async Task GetProductById_WhenNotExists_ReturnsNotFound()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithoutToken_ReturnsUnauthorized()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Producto protegido",
            description = "Solo con JWT",
            price = 15.50m,
            stock = 3
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateUpdateDeleteProduct_WithToken_Works()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        await TestAuthHelper.AuthenticateAsAdminAsync(client);

        var createResponse = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Nuevo Producto Test",
            description = "Creado por test",
            price = 222.99m,
            stock = 9
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductPayload>();
        Assert.NotNull(created);

        var updateResponse = await client.PutAsJsonAsync($"/api/products/{created.Id}", new
        {
            name = "Producto Test Actualizado",
            description = "Actualizado por test",
            price = 230.00m,
            stock = 8
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductPayload>();
        Assert.NotNull(updated);
        Assert.Equal("Producto Test Actualizado", updated.Name);

        var deleteResponse = await client.DeleteAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await client.GetAsync($"/api/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);
    }
}
