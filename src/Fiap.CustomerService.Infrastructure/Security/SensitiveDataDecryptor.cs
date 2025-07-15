using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;

namespace Fiap.CustomerService.Infrastructure.Security
{
    internal class SensitiveDataDecryptor : ISensitiveDataDecryptor
    {
        private readonly IKmsEncryptionService _kmsEncryptionService;

        public SensitiveDataDecryptor(IKmsEncryptionService kmsEncryptionService)
        {
            _kmsEncryptionService = kmsEncryptionService;
        }

        public async Task<Customer> DecryptcustomerAsync(Customer customer)
        {
            if (customer == null) return null!;

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
    }
}