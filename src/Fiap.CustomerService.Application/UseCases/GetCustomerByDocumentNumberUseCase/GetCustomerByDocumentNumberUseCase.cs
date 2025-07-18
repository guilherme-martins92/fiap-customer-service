﻿using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fiap.CustomerService.Application.UseCases.GetCustomerByDocumentNumberUseCase
{
    public class GetCustomerByDocumentNumberUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<GetCustomerByDocumentNumberUseCase> _logger;
        private readonly IHashingService _hashingService;
        private readonly ISensitiveDataEncryptor _sensitiveDataDecryptor;

        public GetCustomerByDocumentNumberUseCase(ICustomerRepository customerRepository, ILogger<GetCustomerByDocumentNumberUseCase> logger, IHashingService hashingService, ISensitiveDataEncryptor sensitiveDataDecryptor)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            _hashingService = hashingService;
            _sensitiveDataDecryptor = sensitiveDataDecryptor;
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

                var customer = await _customerRepository.GetByDocumentNumberlAsync(await _hashingService.HashValue(documentNumber));
                if (customer == null)
                {
                    _logger.LogInformation("No customer found with document number: {DocumentNumber}", documentNumber);
                    return Result<Customer?>.Failure(new List<string> { "Customer not found." });
                }

                customer = await _sensitiveDataDecryptor.DecryptcustomerAsync(customer);

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