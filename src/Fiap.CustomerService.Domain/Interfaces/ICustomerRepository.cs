using Fiap.CustomerService.Domain.Entities;

namespace Fiap.CustomerService.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByDocumentNumberlAsync(string documentNumber);
        Task<Customer?> GetByEmailAsync(string email);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
    }
}