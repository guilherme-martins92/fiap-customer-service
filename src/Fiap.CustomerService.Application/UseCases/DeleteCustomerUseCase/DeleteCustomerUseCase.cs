using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.DeleteCustomerUseCase
{
    public class DeleteCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<DeleteCustomerUseCase> _logger;
        public DeleteCustomerUseCase(ICustomerRepository customerRepository, ILogger<DeleteCustomerUseCase> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }
        public async Task<Result<bool>> ExecuteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogError("Invalid customer ID: {Id}", id);
                    return Result<bool>.Failure(new List<string> { "Invalid customer ID." });
                }

                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                {
                    _logger.LogError("Customer not found with ID: {Id}", id);
                    return Result<bool>.Failure(new List<string> { "Customer not found." });
                }

                await _customerRepository.DeleteAsync(id);
                _logger.LogInformation("Customer deleted successfully with ID: {Id}", id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to delete customer with ID: {Id}", id);
                return Result<bool>.Failure(new List<string> { "An error occurred while deleting the customer." });
            }
        }
    }
}