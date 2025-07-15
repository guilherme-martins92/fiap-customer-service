using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.KeyManagementService;
using Fiap.CustomerService.Domain.Interfaces;
using Fiap.CustomerService.Infrastructure.Repositories;
using Fiap.CustomerService.Infrastructure.Security;
using Fiap.CustomerService.Infrastructure.Security.Fiap.CustomerService.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.CustomerService.Infrastructure.Extensions
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds infrastructure-related services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <param name="configuration">The application configuration used to configure the services.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with the added services.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<ISensitiveDataDecryptor, SensitiveDataDecryptor>();
            services.AddScoped<IKmsEncryptionService, KmsEncryptionService>();

            // Register DynamoDB client
            services.AddAWSService<IAmazonDynamoDB>();

            //Register kms encryption service          
            services.AddAWSService<IAmazonKeyManagementService>();
            services.AddSingleton<IKmsEncryptionService, KmsEncryptionService>();

            // Register DynamoDB context
            services.AddScoped<IDynamoDBContext, DynamoDBContext>();

            return services;
        }
    }
}