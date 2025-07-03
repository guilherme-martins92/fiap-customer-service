using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.GetAllCustomersUseCase
{
    public class GetAllCustomersUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetAllCustomersUseCase> _logger;
        public GetAllCustomersUseCase(ICustomerRepository customerRepository, ILogger<GetAllCustomersUseCase> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }
        public async Task<Result<IEnumerable<Customer>>> ExecuteAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                if (customers == null || !customers.Any())
                {
                    _logger.LogInformation("No customers found in the repository.");
                    return Result<IEnumerable<Customer>>.Failure(new List<string> { "No customers found." });
                }
                return Result<IEnumerable<Customer>>.Success(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customers.");
                throw new InvalidOperationException("An error occurred while retrieving customers.", ex);
            }
        }
    }
}