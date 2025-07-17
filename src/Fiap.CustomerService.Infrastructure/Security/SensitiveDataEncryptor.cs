using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;

namespace Fiap.CustomerService.Infrastructure.Security
{
    public class SensitiveDataEncryptor : ISensitiveDataEncryptor
    {
        private readonly IKmsEncryptionService _kmsEncryptionService;
        private readonly IHashingService _hashingService;

        public SensitiveDataEncryptor(IKmsEncryptionService kmsEncryptionService, IHashingService hashingService)
        {
            _kmsEncryptionService = kmsEncryptionService;
            _hashingService = hashingService;
        }

        public async Task<Customer> DecryptcustomerAsync(Customer customer)
        {
            if (customer is null) return null!;

            customer.DocumentNumber = await _kmsEncryptionService.DecryptAsync(customer.DocumentNumber);
            customer.PhoneNumber = await _kmsEncryptionService.DecryptAsync(customer.PhoneNumber);
            customer.Email = await _kmsEncryptionService.DecryptAsync(customer.Email);
            customer.Street = await _kmsEncryptionService.DecryptAsync(customer.Street);
            customer.HouseNumber = await _kmsEncryptionService.DecryptAsync(customer.HouseNumber);
            customer.City = await _kmsEncryptionService.DecryptAsync(customer.City);
            customer.State = await _kmsEncryptionService.DecryptAsync(customer.State);
            customer.PostalCode = await _kmsEncryptionService.DecryptAsync(customer.PostalCode);
            customer.Country = await _kmsEncryptionService.DecryptAsync(customer.Country);

            return customer;
        }

        public async Task<Customer> EncryptConsumerAsync(Customer customer)
        {
            if (customer is null) return null!;
            customer.DocumentNumber = await _kmsEncryptionService.EncryptAsync(customer.DocumentNumber);
            customer.DocumentNumberHash = await _hashingService.HashValue(customer.DocumentNumber);
            customer.PhoneNumber = await _kmsEncryptionService.EncryptAsync(customer.PhoneNumber);
            customer.Email = await _kmsEncryptionService.EncryptAsync(customer.Email);
            customer.EmailHash = await _hashingService.HashValue(customer.Email);
            customer.Street = await _kmsEncryptionService.EncryptAsync(customer.Street);
            customer.HouseNumber = await _kmsEncryptionService.EncryptAsync(customer.HouseNumber);
            customer.City = await _kmsEncryptionService.EncryptAsync(customer.City);
            customer.State = await _kmsEncryptionService.EncryptAsync(customer.State);
            customer.PostalCode = await _kmsEncryptionService.EncryptAsync(customer.PostalCode);
            customer.Country = await _kmsEncryptionService.EncryptAsync(customer.Country);
            return customer;
        }

        public async Task<IEnumerable<Customer>> DecryptCustomersAsync(IEnumerable<Customer> customers)
        {
            if (customers is null || !customers.Any()) return Enumerable.Empty<Customer>();
            var decryptedCustomers = new List<Customer>();
            foreach (var customer in customers)
            {
                decryptedCustomers.Add(await DecryptcustomerAsync(customer));
            }
            return decryptedCustomers;
        }
    }
}