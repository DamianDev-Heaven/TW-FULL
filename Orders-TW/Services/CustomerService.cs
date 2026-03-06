using Orders_TW.DTOs.Customers;
using Orders_TW.Models;
using Orders_TW.Repositories.Interfaces;
using Orders_TW.Services.Interfaces;
using OrdersTW.Models;

namespace Orders_TW.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            _logger.LogInformation("Se obtuvieron {Count} clientes", customers.Count());
            return customers.Select(MapToDto);
        }

        public async Task<PagedResult<CustomerDto>> GetPagedAsync(int page, int pageSize)
        {
            var pagedCustomers = await _customerRepository.GetPagedAsync(page, pageSize);
            
            _logger.LogInformation(
                "Se obtuvieron {Count} clientes (página {Page} de {TotalPages})", 
                pagedCustomers.Items.Count(), 
                pagedCustomers.Page, 
                pagedCustomers.TotalPages);

            return new PagedResult<CustomerDto>
            {
                Items = pagedCustomers.Items.Select(MapToDto),
                Page = pagedCustomers.Page,
                PageSize = pagedCustomers.PageSize,
                TotalCount = pagedCustomers.TotalCount
            };
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer == null ? null : MapToDto(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            // Validar email único
            var existingCustomer = await _customerRepository.GetByEmailAsync(dto.Email);
            if (existingCustomer != null)
            {
                throw new ArgumentException($"Ya existe un cliente con el email '{dto.Email}'");
            }

            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email
            };

            var created = await _customerRepository.CreateAsync(customer);
            return MapToDto(created);
        }

        private static CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email
            };
        }
    }
}