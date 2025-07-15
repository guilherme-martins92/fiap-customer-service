using Fiap.CustomerService.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Fiap.CustomerService.Infrastructure.Security
{    public class HashingService : IHashingService
    {
        private const string Salt = "FiapCustomerSalt2024";

        public async Task<string> HashValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            using var sha256 = SHA256.Create();
            var saltedDocument = $"{Salt}{value}";
            var bytes = Encoding.UTF8.GetBytes(saltedDocument);
            var hashBytes = await Task.Run(() => sha256.ComputeHash(bytes));
            return Convert.ToBase64String(hashBytes);
        }
    }
}