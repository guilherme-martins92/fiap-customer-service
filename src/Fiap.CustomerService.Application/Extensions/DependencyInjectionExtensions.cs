using Fiap.CustomerService.Application.UseCases.CreateCustomerUseCase;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.CustomerService.Application.Extensions
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds application-specific services and dependencies to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>This method registers the necessary services and validators required for the
        /// application's use cases. It is typically called during application startup to configure the dependency
        /// injection container.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance, allowing for method chaining.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<CreateCustomerUseCase>();
            services.AddScoped<IValidator<CreateCustomerInput>, CreateCustomerValidator>();
            return services;
        }
    }
}