using Moq;
using Orders_TW.DTOs.Orders;
using Orders_TW.Repositories.Interfaces;
using Orders_TW.Services;
using OrdersTW.Models;
using Microsoft.Extensions.Logging;

namespace Orders_TW.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _service = new OrderService(
                _mockOrderRepository.Object,
                _mockCustomerRepository.Object,
                _mockProductRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ValidOrder_CalculatesTotal()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerId = 1,
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = 1, Quantity = 2 },
                    new CreateOrderItemDto { ProductId = 2, Quantity = 1 }
                }
            };

            _mockCustomerRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockProductRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 10 });
            _mockProductRepository.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(new Product { Id = 2, Name = "Product 2", Price = 5.99m, Stock = 10 });

            var createdOrder = new Order
            {
                Id = 1,
                CustomerId = 1,
                Total = 27.97m,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 10.99m },
                    new OrderItem { ProductId = 2, Quantity = 1, UnitPrice = 5.99m }
                },
                Customer = new Customer { Id = 1, FullName = "Test Customer", Email = "test@test.com" }
            };

            _mockOrderRepository.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder);
            _mockOrderRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(createdOrder);
            _mockProductRepository.Setup(r => r.UpdateStockAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(27.97m, result.Total);
            Assert.Equal(2, result.OrderItems.Count);
            _mockProductRepository.Verify(r => r.UpdateStockAsync(1, 2), Times.Once);
            _mockProductRepository.Verify(r => r.UpdateStockAsync(2, 1), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_InsufficientStock_ThrowsException()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerId = 1,
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = 1, Quantity = 20 }
                }
            };

            _mockCustomerRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockProductRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Product { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 5 });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_NonExistentCustomer_ThrowsException()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerId = 999,
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = 1, Quantity = 1 }
                }
            };

            _mockCustomerRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_NonExistentProduct_ThrowsException()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerId = 1,
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto { ProductId = 999, Quantity = 1 }
                }
            };

            _mockCustomerRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockProductRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_EmptyOrderItems_ThrowsException()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerId = 1,
                OrderItems = new List<CreateOrderItemDto>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }
    }
}
