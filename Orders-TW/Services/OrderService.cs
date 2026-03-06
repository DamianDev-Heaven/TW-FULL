using Orders_TW.DTOs.Orders;
using Orders_TW.Models;
using Orders_TW.Repositories.Interfaces;
using Orders_TW.Services.Interfaces;
using OrdersTW.Models;

namespace Orders_TW.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                _logger.LogInformation("Se obtuvieron {Count} órdenes", orders.Count());
                return orders.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las órdenes");
                throw;
            }
        }

        public async Task<PagedResult<OrderDto>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var pagedOrders = await _orderRepository.GetPagedAsync(page, pageSize);
                
                _logger.LogInformation(
                    "Se obtuvieron {Count} órdenes (página {Page} de {TotalPages})", 
                    pagedOrders.Items.Count(), 
                    pagedOrders.Page, 
                    pagedOrders.TotalPages);

                return new PagedResult<OrderDto>
                {
                    Items = pagedOrders.Items.Select(MapToDto),
                    Page = pagedOrders.Page,
                    PageSize = pagedOrders.PageSize,
                    TotalCount = pagedOrders.TotalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener órdenes paginadas");
                throw;
            }
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                
                if (order == null)
                {
                    _logger.LogWarning("No se encontró la orden con Id: {OrderId}", id);
                    return null;
                }

                return MapToDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la orden con Id: {OrderId}", id);
                throw;
            }
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            try
            {
                if (dto.OrderItems == null || !dto.OrderItems.Any())
                {
                    throw new ArgumentException("La orden debe contener al menos un producto");
                }

                var customerExists = await _customerRepository.ExistsAsync(dto.CustomerId);
                if (!customerExists)
                {
                    _logger.LogWarning("Intento de crear orden con cliente inexistente. CustomerId: {CustomerId}", dto.CustomerId);
                    throw new KeyNotFoundException($"No existe el cliente con Id {dto.CustomerId}");
                }

                var orderItems = new List<OrderItem>();
                decimal total = 0;

                foreach (var item in dto.OrderItems)
                {
                    if (item.Quantity <= 0)
                    {
                        throw new ArgumentException($"La cantidad del producto debe ser mayor a 0");
                    }

                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    
                    if (product == null)
                    {
                        _logger.LogWarning("Intento de crear orden con producto inexistente. ProductId: {ProductId}", item.ProductId);
                        throw new KeyNotFoundException($"No existe el producto con Id {item.ProductId}");
                    }

                    if (product.Stock < item.Quantity)
                    {
                        _logger.LogWarning(
                            "Stock insuficiente. ProductId: {ProductId}, Stock disponible: {Stock}, Cantidad solicitada: {Quantity}",
                            item.ProductId, product.Stock, item.Quantity);
                        
                        throw new ArgumentException(
                            $"Stock insuficiente para el producto '{product.Name}'. " +
                            $"Stock disponible: {product.Stock}, Cantidad solicitada: {item.Quantity}");
                    }

                    var subtotal = product.Price * item.Quantity;
                    total += subtotal;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }

                var order = new Order
                {
                    CustomerId = dto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    Total = total,
                    OrderItems = orderItems
                };

                var createdOrder = await _orderRepository.CreateAsync(order);

                foreach (var item in dto.OrderItems)
                {
                    await _productRepository.UpdateStockAsync(item.ProductId, item.Quantity);
                }

                _logger.LogInformation(
                    "Orden creada exitosamente. OrderId: {OrderId}, CustomerId: {CustomerId}, Total: {Total}",
                    createdOrder.Id, dto.CustomerId, total);

                var fullOrder = await _orderRepository.GetByIdAsync(createdOrder.Id);
                return MapToDto(fullOrder!);
            }
            catch (Exception ex) when (ex is not KeyNotFoundException && ex is not ArgumentException)
            {
                _logger.LogError(ex, "Error al crear la orden para el cliente: {CustomerId}", dto.CustomerId);
                throw;
            }
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FullName ?? string.Empty,
                OrderDate = order.OrderDate,
                Total = order.Total,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}