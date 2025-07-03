using Fiap.CustomerService.Domain.Entities;

namespace Fiap.CustomerService.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task UpdateAsync(Customer customer);

        Task DeleteAsync(int id);
    }
}