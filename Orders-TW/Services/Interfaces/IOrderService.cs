using Orders_TW.DTOs.Orders;
using Orders_TW.Models;

namespace Orders_TW.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<PagedResult<OrderDto>> GetPagedAsync(int page, int pageSize);
        Task<OrderDto?> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
    }
}
