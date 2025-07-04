using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Fiap.CustomerService.Domain.Interfaces;
using Fiap.CustomerService.Infrastructure.Repositories;
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

            // Register DynamoDB client
            services.AddAWSService<IAmazonDynamoDB>();

            // Register DynamoDB context
            services.AddScoped<IDynamoDBContext, DynamoDBContext>();

            return services;
        }
    }
}