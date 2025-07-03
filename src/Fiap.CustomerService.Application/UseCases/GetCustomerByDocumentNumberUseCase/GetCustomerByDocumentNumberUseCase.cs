using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.GetCustomerByDocumentNumberUseCase
{
    public class GetCustomerByDocumentNumberUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomerByDocumentNumberUseCase> _logger;
        public GetCustomerByDocumentNumberUseCase(ICustomerRepository customerRepository, ILogger<GetCustomerByDocumentNumberUseCase> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }
        public async Task<Result<Customer?>> ExecuteAsync(string documentNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                {
                    _logger.LogError("Document number is null or empty.");
                    return Result<Customer?>.Failure(new List<string> { "Document number cannot be null or empty." });
                }

                var customer = await _customerRepository.GetByDocumentNumberlAsync(documentNumber);
                if (customer == null)
                {
                    _logger.LogInformation("No customer found with document number: {DocumentNumber}", documentNumber);
                    return Result<Customer?>.Failure(new List<string> { "Customer not found." });
                }

                _logger.LogInformation("Customer found with document number: {DocumentNumber}", documentNumber);
                return Result<Customer?>.Success(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the customer by document number: {DocumentNumber}", documentNumber);
                throw new InvalidOperationException("An error occurred while retrieving the customer by document number.", ex);
            }
        }
    }
}