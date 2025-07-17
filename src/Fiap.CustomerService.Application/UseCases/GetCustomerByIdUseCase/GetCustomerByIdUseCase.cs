using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Application.DTOs;
using Fiap.CustomerService.Application.Mappings;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.GetCustomerByIdUseCase
{
    public class GetCustomerByIdUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomerByIdUseCase> _logger;
        private readonly ISensitiveDataEncryptor _sensitiveDataDecryptor;
        public GetCustomerByIdUseCase(ICustomerRepository customerRepository, ILogger<GetCustomerByIdUseCase> logger, ISensitiveDataEncryptor sensitiveDataDecryptor)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _sensitiveDataDecryptor = sensitiveDataDecryptor;
        }

        public async Task<Result<CustomerOutputDto>> ExecuteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogError("Invalid customer ID: {Id}", id);
                    return Result<CustomerOutputDto>.Failure(new List<string> { "Invalid customer ID." });
                }

                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    _logger.LogWarning("Customer not found with ID: {Id}", id);
                    return Result<CustomerOutputDto>.Failure(new List<string> { "Customer not found." });
                }

                customer = await _sensitiveDataDecryptor.DecryptcustomerAsync(customer);

                _logger.LogInformation("Customer retrieved successfully with ID: {Id}", id);
                return Result<CustomerOutputDto>.Success(CustomerMapper.FromEntity(customer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customer with ID: {Id}", id);
                return Result<CustomerOutputDto>.Failure(new List<string> { "An error occurred while retrieving the customer." });
            }
        }
    }
}