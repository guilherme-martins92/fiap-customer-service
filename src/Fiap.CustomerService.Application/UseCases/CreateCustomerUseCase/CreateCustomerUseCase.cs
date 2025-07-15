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
        private readonly IKmsEncryptionService _kmsEncryptionService;
        private readonly IHashingService _hashingService;

        public CreateCustomerUseCase(ICustomerRepository customerRepository, IValidator<CreateCustomerInput> validator, ILogger<CreateCustomerUseCase> logger, IKmsEncryptionService kmsEncryptionService, IHashingService hashingService)
        {
            _customerRepository = customerRepository;
            _validator = validator;
            _logger = logger;
            _kmsEncryptionService = kmsEncryptionService;
            _hashingService = hashingService;
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

            var duplicateErrors = await CheckForDuplicatesAsync(customer);
            if (duplicateErrors is not null)
                return duplicateErrors;

            var newCustomer = await BuildCustomerAsync(customer);

            await _customerRepository.AddAsync(newCustomer);

            _logger.LogInformation("Customer created successfully with ID: {CustomerId}", newCustomer.Id);
            return Result<Customer>.Success(newCustomer);
        }

        private async Task<Result<Customer>?> CheckForDuplicatesAsync(CreateCustomerInput customer)
        {
            if (await _customerRepository.GetByDocumentNumberlAsync(await _hashingService.HashValue(FormatUtils.UnformatDocumentNumber(customer.DocumentNumber))) is not null)
            {
                _logger.LogWarning("Duplicate document: {DocumentNumber}", customer.DocumentNumber);
                return Result<Customer>.Failure(["Customer with this document number already exists."]);
            }

            if (await _customerRepository.GetByEmailAsync(await _hashingService.HashValue(customer.Email)) is not null)
            {
                _logger.LogWarning("Duplicate email: {Email}", customer.Email);
                return Result<Customer>.Failure(["Customer with this email already exists."]);
            }

            return null;
        }

        private async Task<Customer> BuildCustomerAsync(CreateCustomerInput customer)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DocumentNumber = await _kmsEncryptionService.EncryptAsync(FormatUtils.UnformatDocumentNumber(customer.DocumentNumber)),
                DocumentNumberHash = await _hashingService.HashValue(customer.DocumentNumber),
                DateOfBirth = customer.DateOfBirth,
                Email = await _kmsEncryptionService.EncryptAsync(customer.Email),
                EmailHash = await _hashingService.HashValue(customer.Email),
                PhoneNumber = await _kmsEncryptionService.EncryptAsync(FormatUtils.UnformatPhoneNumber(customer.PhoneNumber)),
                Street = await _kmsEncryptionService.EncryptAsync(customer.Street),
                HouseNumber = await _kmsEncryptionService.EncryptAsync(customer.HouseNumber),
                City = await _kmsEncryptionService.EncryptAsync(customer.City),
                State = await _kmsEncryptionService.EncryptAsync(customer.State),
                PostalCode = await _kmsEncryptionService.EncryptAsync(FormatUtils.UnformatPostalCode(customer.PostalCode)),
                Country = await _kmsEncryptionService.EncryptAsync(customer.Country),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}