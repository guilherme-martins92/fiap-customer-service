namespace Fiap.CustomerService.Domain.Interfaces
{
    public interface IKmsEncryptionService
    {
        Task<string> EncryptAsync(string plaintext);
        Task<string> DecryptAsync(string ciphertext);
    }
}