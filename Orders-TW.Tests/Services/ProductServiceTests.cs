using Moq;
using Orders_TW.DTOs.Products;
using Orders_TW.Repositories.Interfaces;
using Orders_TW.Services;
using OrdersTW.Models;
using Microsoft.Extensions.Logging;

namespace Orders_TW.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10.99m, Stock = 5 },
                new Product { Id = 2, Name = "Product 2", Price = 20.99m, Stock = 10 }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product", Price = 10.99m, Stock = 5 };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Name);
            Assert.Equal(10.99m, result.Price);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "New Product",
                Description = "Test Description",
                Price = 15.99m,
                Stock = 10
            };

            var createdProduct = new Product
            {
                Id = 1,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            Assert.Equal(15.99m, result.Price);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ExistingProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var dto = new UpdateProductDto
            {
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 25.99m,
                Stock = 15
            };

            var existingProduct = new Product { Id = 1, Name = "Old Name", Price = 10m, Stock = 5 };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingProduct);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(existingProduct);

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Product", result.Name);
            Assert.Equal(25.99m, result.Price);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingProduct_ReturnsNull()
        {
            // Arrange
            var dto = new UpdateProductDto { Name = "Test", Price = 10m, Stock = 5 };
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.UpdateAsync(999, dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingProduct_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingProduct_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
