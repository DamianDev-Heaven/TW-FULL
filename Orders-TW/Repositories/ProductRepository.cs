using Microsoft.EntityFrameworkCore;
using Orders_TW.Data;
using Orders_TW.Models;
using Orders_TW.Repositories.Interfaces;
using OrdersTW.Models;

namespace Orders_TW.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await _context.Products
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos de la base de datos");
                throw;
            }
        }

        public async Task<PagedResult<Product>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var totalCount = await _context.Products.CountAsync();
                
                var items = await _context.Products
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<Product>
                {
                    Items = items,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos paginados");
                throw;
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar producto con Id: {ProductId}", id);
                throw;
            }
        }

        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al crear producto: {Name}", product.Name);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear producto");
                throw;
            }
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            try
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return product;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al actualizar producto con Id: {ProductId}", product.Id);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al actualizar producto con Id: {ProductId}", product.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar producto");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar producto con Id: {ProductId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar producto");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.Products.AnyAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de producto con Id: {ProductId}", id);
                throw;
            }
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Intento de actualizar stock de producto inexistente. ProductId: {ProductId}", productId);
                    throw new KeyNotFoundException($"Producto con Id {productId} no encontrado");
                }

                if (product.Stock < quantity)
                {
                    _logger.LogWarning(
                        "Stock insuficiente al intentar actualizar. ProductId: {ProductId}, Stock: {Stock}, Cantidad: {Quantity}",
                        productId, product.Stock, quantity);
                    throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");
                }

                product.Stock -= quantity;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is not KeyNotFoundException && ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error al actualizar stock del producto con Id: {ProductId}", productId);
                throw;
            }
        }
    }
}
