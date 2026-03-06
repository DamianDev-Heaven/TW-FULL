namespace Orders_TW.Tests;

public class LoginResponsePayload
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class PagedResultPayload<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}

public class ProductPayload
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class CustomerPayload
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class OrderPayload
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemPayload> OrderItems { get; set; } = new();
}

public class OrderItemPayload
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
