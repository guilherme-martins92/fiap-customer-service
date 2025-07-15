using Fiap.CustomerService.Domain.Entities;

namespace Fiap.CustomerService.Domain.Interfaces
{
    public interface ISensitiveDataDecryptor
    {
        Task<Customer> DecryptcustomerAsync(Customer customer);
    }
}