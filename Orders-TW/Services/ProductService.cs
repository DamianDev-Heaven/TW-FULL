using Orders_TW.DTOs.Products;
using Orders_TW.Models;
using Orders_TW.Repositories.Interfaces;
using Orders_TW.Services.Interfaces;
using OrdersTW.Models;

namespace Orders_TW.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                _logger.LogInformation("Se obtuvieron {Count} productos", products.Count());
                return products.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                throw;
            }
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var pagedProducts = await _productRepository.GetPagedAsync(page, pageSize);
                
                _logger.LogInformation(
                    "Se obtuvieron {Count} productos (página {Page} de {TotalPages})", 
                    pagedProducts.Items.Count(), 
                    pagedProducts.Page, 
                    pagedProducts.TotalPages);

                return new PagedResult<ProductDto>
                {
                    Items = pagedProducts.Items.Select(MapToDto),
                    Page = pagedProducts.Page,
                    PageSize = pagedProducts.PageSize,
                    TotalCount = pagedProducts.TotalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos paginados");
                throw;
            }
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                
                if (product == null)
                {
                    _logger.LogWarning("No se encontró el producto con Id: {ProductId}", id);
                    return null;
                }

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con Id: {ProductId}", id);
                throw;
            }
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            try
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Stock = dto.Stock
                };

                var created = await _productRepository.CreateAsync(product);
                _logger.LogInformation("Producto creado exitosamente con Id: {ProductId}, Nombre: {Name}", 
                    created.Id, created.Name);
                
                return MapToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto: {Name}", dto.Name);
                throw;
            }
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                
                if (product == null)
                {
                    _logger.LogWarning("Intento de actualizar producto inexistente con Id: {ProductId}", id);
                    return null;
                }

                product.Name = dto.Name;
                product.Price = dto.Price;
                product.Stock = dto.Stock;

                var updated = await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Producto actualizado con Id: {ProductId}", id);
                
                return MapToDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto con Id: {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var result = await _productRepository.DeleteAsync(id);
                
                if (result)
                {
                    _logger.LogInformation("Producto eliminado con Id: {ProductId}", id);
                }
                else
                {
                    _logger.LogWarning("Intento de eliminar producto inexistente con Id: {ProductId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con Id: {ProductId}", id);
                throw;
            }
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            };
        }
    }
}