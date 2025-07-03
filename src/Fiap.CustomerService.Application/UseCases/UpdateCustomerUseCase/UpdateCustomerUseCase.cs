using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.UpdateCustomerUseCase
{
    public class UpdateCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<UpdateCustomerUseCase> _logger;
        private readonly IValidator<UpdateCustomerInput> _validator;

        public UpdateCustomerUseCase(ICustomerRepository customerRepository, ILogger<UpdateCustomerUseCase> logger, IValidator<UpdateCustomerInput> validator)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result<Customer>> ExecuteAsync(int id, UpdateCustomerInput updateCustomerInput)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(updateCustomerInput);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateCustomerUseCase: {@Errors}", validationResult.Errors);
                    return Result<Customer>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                {
                    _logger.LogWarning("Customer with ID {Id} not found", id);
                    return Result<Customer>.Failure(new List<string> { "Customer not found" });
                }

                existingCustomer.FirstName = updateCustomerInput.FirstName;
                existingCustomer.LastName = updateCustomerInput.LastName;
                existingCustomer.DateOfBirth = updateCustomerInput.DateOfBirth;
                existingCustomer.Email = updateCustomerInput.Email;
                existingCustomer.PhoneNumber = updateCustomerInput.PhoneNumber;
                existingCustomer.Street = updateCustomerInput.Street;
                existingCustomer.HouseNumber = updateCustomerInput.HouseNumber;
                existingCustomer.City = updateCustomerInput.City;
                existingCustomer.State = updateCustomerInput.State;
                existingCustomer.PostalCode = updateCustomerInput.PostalCode;
                existingCustomer.Country = updateCustomerInput.Country;
                existingCustomer.UpdatedAt = DateTime.UtcNow;
                await _customerRepository.UpdateAsync(existingCustomer);

                _logger.LogInformation("Successfully updated customer with ID {Id}", existingCustomer.Id);
                return Result<Customer>.Success(existingCustomer);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing UpdateCustomerUseCase");
                return Result<Customer>.Failure(new List<string> { "An unexpected error occurred" });
            }  
        }
    }
}