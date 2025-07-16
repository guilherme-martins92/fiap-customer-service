using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Application.DTOs;
using Fiap.CustomerService.Application.Mappings;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.CreateCustomerUseCase
{
    public class CreateCustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IValidator<CustomerInputDto> _validator;
        private readonly ILogger<CreateCustomerUseCase> _logger;
        private readonly IKmsEncryptionService _kmsEncryptionService;
        private readonly IHashingService _hashingService;
        private readonly ISensitiveDataEncryptor _sensitiveDataDecryptor;

        public CreateCustomerUseCase(ICustomerRepository customerRepository, IValidator<CustomerInputDto> validator, ILogger<CreateCustomerUseCase> logger, IKmsEncryptionService kmsEncryptionService, IHashingService hashingService, ISensitiveDataEncryptor sensitiveDataDecryptor)
        {
            _customerRepository = customerRepository;
            _validator = validator;
            _logger = logger;
            _kmsEncryptionService = kmsEncryptionService;
            _hashingService = hashingService;
            _sensitiveDataDecryptor = sensitiveDataDecryptor;
        }

        public async Task<Result<CustomerOutputDto>> ExecuteAsync(CustomerInputDto customer)
        {
            if (customer == null)
            {
                _logger.LogError("Customer input is null.");
                return Result<CustomerOutputDto>.Failure(new List<string> { "Customer input cannot be null." });
            }

            var validationResult = await _validator.ValidateAsync(customer);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed: {Errors}", validationResult.Errors);
                return Result<CustomerOutputDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var duplicateErrors = await CheckForDuplicatesAsync(customer);
            if (duplicateErrors is not null)
                return duplicateErrors;

            var newCustomer = CustomerMapper.ToEntity(customer);
            newCustomer = await _sensitiveDataDecryptor.EncryptConsumerAsync(newCustomer);

            await _customerRepository.AddAsync(newCustomer);

            newCustomer = await _sensitiveDataDecryptor.DecryptcustomerAsync(newCustomer);

            _logger.LogInformation("Customer created successfully with ID: {CustomerId}", newCustomer.Id);
            return Result<CustomerOutputDto>.Success(CustomerMapper.FromEntity(newCustomer));
        }

        private async Task<Result<CustomerOutputDto>?> CheckForDuplicatesAsync(CustomerInputDto customer)
        {
            if (await _customerRepository.GetByDocumentNumberlAsync(await _hashingService.HashValue(FormatUtils.UnformatDocumentNumber(customer.DocumentNumber))) is not null)
            {
                _logger.LogWarning("Duplicate document: {DocumentNumber}", customer.DocumentNumber);
                return Result<CustomerOutputDto>.Failure(["Customer with this document number already exists."]);
            }

            if (await _customerRepository.GetByEmailAsync(await _hashingService.HashValue(customer.Email)) is not null)
            {
                _logger.LogWarning("Duplicate email: {Email}", customer.Email);
                return Result<CustomerOutputDto>.Failure(["Customer with this email already exists."]);
            }

            return null;
        }
    }
}