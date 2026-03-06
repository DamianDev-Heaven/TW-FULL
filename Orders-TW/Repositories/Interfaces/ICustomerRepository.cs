using Orders_TW.Models;
using OrdersTW.Models;

namespace Orders_TW.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer> CreateAsync(Customer customer);
        Task<bool> ExistsAsync(int id);
    }
}
