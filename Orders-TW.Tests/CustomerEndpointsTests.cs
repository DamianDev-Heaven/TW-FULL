using System.Net;
using System.Net.Http.Json;
using Orders_TW.Tests.Infrastructure;

namespace Orders_TW.Tests;

public class CustomerEndpointsTests
{
    [Fact]
    public async Task GetCustomersPaged_ReturnsOkWithPagination()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/customers?page=1&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PagedResultPayload<CustomerPayload>>();
        Assert.NotNull(payload);
        Assert.Equal(1, payload.Page);
        Assert.Equal(5, payload.PageSize);
        Assert.True(payload.Items.Count <= 5);
        Assert.True(payload.TotalCount > 0);
    }

    [Fact]
    public async Task GetCustomersAll_ReturnsOk()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/customers/all");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<CustomerPayload>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
    }

    [Fact]
    public async Task GetCustomerById_WhenExists_ReturnsOk()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/customers/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CustomerPayload>();
        Assert.NotNull(payload);
        Assert.Equal(1, payload.Id);
    }

    [Fact]
    public async Task GetCustomerById_WhenNotExists_ReturnsNotFound()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/customers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithoutToken_ReturnsUnauthorized()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "Cliente Sin Token",
            email = "cliente.sintoken@test.com"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithToken_ReturnsCreated()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        await TestAuthHelper.AuthenticateAsAdminAsync(client);

        var response = await client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "Cliente Con Token",
            email = "cliente.contoken@test.com"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CustomerPayload>();
        Assert.NotNull(payload);
        Assert.Equal("Cliente Con Token", payload.FullName);
    }
}
