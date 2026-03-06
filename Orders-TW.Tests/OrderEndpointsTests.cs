using System.Net;
using System.Net.Http.Json;
using Orders_TW.Tests.Infrastructure;

namespace Orders_TW.Tests;

public class OrderEndpointsTests
{
    [Fact]
    public async Task GetOrdersPaged_ReturnsOkWithPagination()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders?page=1&pageSize=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PagedResultPayload<OrderPayload>>();
        Assert.NotNull(payload);
        Assert.Equal(1, payload.Page);
        Assert.Equal(3, payload.PageSize);
        Assert.True(payload.Items.Count <= 3);
        Assert.True(payload.TotalCount > 0);
    }

    [Fact]
    public async Task GetOrdersAll_ReturnsOk()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders/all");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<OrderPayload>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
    }

    [Fact]
    public async Task GetOrderById_WhenExists_ReturnsOk()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<OrderPayload>();
        Assert.NotNull(payload);
        Assert.Equal(1, payload.Id);
        Assert.NotEmpty(payload.OrderItems);
    }

    [Fact]
    public async Task GetOrderById_WhenNotExists_ReturnsNotFound()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WithoutToken_ReturnsUnauthorized()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/orders", new
        {
            customerId = 1,
            orderItems = new[]
            {
                new { productId = 1, quantity = 1 }
            }
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WithToken_CalculatesTotal_AndReducesStock()
    {
        using var factory = new ApiWebApplicationFactory();
        using var client = factory.CreateClient();
        await TestAuthHelper.AuthenticateAsAdminAsync(client);

        var productBeforeResponse = await client.GetAsync("/api/products/1");
        productBeforeResponse.EnsureSuccessStatusCode();
        var productBefore = await productBeforeResponse.Content.ReadFromJsonAsync<ProductPayload>();
        Assert.NotNull(productBefore);

        var orderResponse = await client.PostAsJsonAsync("/api/orders", new
        {
            customerId = 1,
            orderItems = new[]
            {
                new { productId = 1, quantity = 2 }
            }
        });

        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);

        var orderCreated = await orderResponse.Content.ReadFromJsonAsync<OrderPayload>();
        Assert.NotNull(orderCreated);
        Assert.Single(orderCreated.OrderItems);
        Assert.Equal(orderCreated.OrderItems[0].UnitPrice * orderCreated.OrderItems[0].Quantity, orderCreated.Total);

        var productAfterResponse = await client.GetAsync("/api/products/1");
        productAfterResponse.EnsureSuccessStatusCode();
        var productAfter = await productAfterResponse.Content.ReadFromJsonAsync<ProductPayload>();
        Assert.NotNull(productAfter);
        Assert.Equal(productBefore.Stock - 2, productAfter.Stock);
    }
}
