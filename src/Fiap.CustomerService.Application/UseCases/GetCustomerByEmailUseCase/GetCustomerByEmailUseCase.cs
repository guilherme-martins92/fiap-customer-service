using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.GetCustomerByEmailUseCase
{
    public class GetCustomerByEmailUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomerByEmailUseCase> _logger;

        public GetCustomerByEmailUseCase(ICustomerRepository customerRepository, ILogger<GetCustomerByEmailUseCase> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<Result<Customer>> ExecuteAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return Result<Customer>.Failure(new List<string> { "Email is required." });
                }
                var customer = await _customerRepository.GetByEmailAsync(email);
                if (customer == null)
                {
                    return Result<Customer>.Failure(new List<string> { "Customer not found." });
                }
                return Result<Customer>.Success(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the customer by email: {Email}", email);
                throw new InvalidOperationException("An error occurred while retrieving the customer by email.", ex);
            }
        }
    }
}