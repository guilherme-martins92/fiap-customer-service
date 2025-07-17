using Fiap.CustomerService.Domain.Entities;

namespace Fiap.CustomerService.Domain.Interfaces
{
    public interface ISensitiveDataEncryptor
    {
        Task<Customer> DecryptcustomerAsync(Customer customer);
        Task<Customer> EncryptConsumerAsync(Customer customer);
        Task<IEnumerable<Customer>> DecryptCustomersAsync(IEnumerable<Customer> customers);
    }
}