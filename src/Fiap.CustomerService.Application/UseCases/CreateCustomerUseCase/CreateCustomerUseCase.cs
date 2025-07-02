using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.CreateCustomerUseCase
{
    public class CreateCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IValidator<CreateCustomerInput> _validator;
        private readonly ILogger<CreateCustomerUseCase> _logger;

        public CreateCustomerUseCase(ICustomerRepository customerRepository, IValidator<CreateCustomerInput> validator, ILogger<CreateCustomerUseCase> logger)
        {
            _customerRepository = customerRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Customer>> ExecuteAsync(CreateCustomerInput customer)
        {
            if (customer == null)
            {
                _logger.LogError("Customer input is null.");
                return Result<Customer>.Failure(new List<string> { "Customer input cannot be null." });
            }

            var validationResult = await _validator.ValidateAsync(customer);

            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed: {Errors}", validationResult.Errors);
                return Result<Customer>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var newCustomer = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DocumentNumber = customer.DocumentNumber,
                DateOfBirth = customer.DateOfBirth,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Street = customer.Street,
                HouseNumber = customer.HouseNumber,
                City = customer.City,
                State = customer.State,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _customerRepository.AddAsync(newCustomer);

            _logger.LogInformation("Customer created successfully with ID: {CustomerId}", newCustomer.Id);
            return Result<Customer>.Success(newCustomer);
        }
    }
}