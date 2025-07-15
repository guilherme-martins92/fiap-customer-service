using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Fiap.CustomerService.Domain.Interfaces;
using Fiap.CustomerService.Infrastructure.AwsServices;
using Microsoft.Extensions.Options;
using System.Text;

namespace Fiap.CustomerService.Infrastructure.Security
{

    namespace Fiap.CustomerService.Infrastructure.Security
    {
        public class KmsEncryptionService : IKmsEncryptionService
        {
            private readonly IAmazonKeyManagementService _kmsClient;
            private readonly AwsSettings _settings;

            public KmsEncryptionService(IAmazonKeyManagementService kmsClient, IOptions<AwsSettings> settings)
            {
                _kmsClient = kmsClient;
                _settings = settings.Value;
            }

            public async Task<string> EncryptAsync(string plaintext)
            {
                if (string.IsNullOrWhiteSpace(_settings.KmsKeyId))
                    throw new InvalidOperationException("A chave KMS não está configurada.");

                var request = new EncryptRequest
                {
                    KeyId = _settings.KmsKeyId,
                    Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(plaintext))
                };

                var response = await _kmsClient.EncryptAsync(request);
                return Convert.ToBase64String(response.CiphertextBlob.ToArray());
            }

            public async Task<string> DecryptAsync(string ciphertext)
            {
                var request = new DecryptRequest
                {
                    CiphertextBlob = new MemoryStream(Convert.FromBase64String(ciphertext))
                };

                var response = await _kmsClient.DecryptAsync(request);
                return Encoding.UTF8.GetString(response.Plaintext.ToArray());
            }
        }
    }
}