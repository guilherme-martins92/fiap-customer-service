using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Application.DTOs;
using Fiap.CustomerService.Application.Mappings;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.GetAllCustomersUseCase
{
    public class GetAllCustomersUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetAllCustomersUseCase> _logger;
        private readonly ISensitiveDataEncryptor _sensitiveDataDecryptor;
        public GetAllCustomersUseCase(ICustomerRepository customerRepository, ILogger<GetAllCustomersUseCase> logger, ISensitiveDataEncryptor sensitiveDataDecryptor)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _sensitiveDataDecryptor = sensitiveDataDecryptor;
        }
        public async Task<Result<IEnumerable<CustomerOutputDto>>> ExecuteAsync()
        {
            try
            {
                var customersOutputDto = new List<CustomerOutputDto>();

                var customers = await _customerRepository.GetAllAsync();
                if (customers == null || !customers.Any())
                {
                    _logger.LogInformation("No customers found in the repository.");
                    return Result<IEnumerable<CustomerOutputDto>>.Failure(new List<string> { "No customers found." });
                }

                customers = await _sensitiveDataDecryptor.DecryptCustomersAsync(customers);
                
                foreach (var customer in customers)
                {
                    if (customer == null)
                    {
                        _logger.LogWarning("Found a null customer in the repository.");
                        continue;
                    } 
                    customersOutputDto.Add(CustomerMapper.FromEntity(customer));
                }

                return Result<IEnumerable<CustomerOutputDto>>.Success(customersOutputDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customers.");
                throw new InvalidOperationException("An error occurred while retrieving customers.", ex);
            }
        }
    }
}