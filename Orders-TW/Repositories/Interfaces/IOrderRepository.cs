using Orders_TW.Models;
using OrdersTW.Models;

namespace Orders_TW.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<PagedResult<Order>> GetPagedAsync(int page, int pageSize);
        Task<Order?> GetByIdAsync(int id);
        Task<Order> CreateAsync(Order order);
    }
}
