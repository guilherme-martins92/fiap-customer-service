using Fiap.CustomerService.Application.UseCases.GetCustomerByDocumentNumberUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Fiap.CustomerService.Application.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class GetCustomerByDocumentNumberUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<GetCustomerByDocumentNumberUseCase>> _loggerMock;
        private readonly GetCustomerByDocumentNumberUseCase _useCase;

        public GetCustomerByDocumentNumberUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<GetCustomerByDocumentNumberUseCase>>();
            _useCase = new GetCustomerByDocumentNumberUseCase(
                _customerRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenDocumentNumberIsNullOrEmpty()
        {
            // Act
            var resultNull = await _useCase.ExecuteAsync(null!);
            var resultEmpty = await _useCase.ExecuteAsync("");
            var resultWhitespace = await _useCase.ExecuteAsync("   ");

            // Assert
            Assert.False(resultNull.IsSuccess);
            Assert.Contains("Document number cannot be null or empty.", resultNull.Errors);

            Assert.False(resultEmpty.IsSuccess);
            Assert.Contains("Document number cannot be null or empty.", resultEmpty.Errors);

            Assert.False(resultWhitespace.IsSuccess);
            Assert.Contains("Document number cannot be null or empty.", resultWhitespace.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenCustomerNotFound()
        {
            // Arrange
            var documentNumber = "123456789";
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync(documentNumber))
                .ReturnsAsync((Customer?)null);

            // Act
            var result = await _useCase.ExecuteAsync(documentNumber);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer not found.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenCustomerFound()
        {
            // Arrange
            var documentNumber = "123456789";
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DocumentNumber = documentNumber,
                DateOfBirth = new DateTime(1985, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                Email = "john.doe@example.com",
                PhoneNumber = "555-0000",
                Street = "Main St",
                HouseNumber = "1",
                City = "Metropolis",
                State = "NY",
                PostalCode = "10001",
                Country = "USA",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync(documentNumber))
                .ReturnsAsync(customer);

            // Act
            var result = await _useCase.ExecuteAsync(documentNumber);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(customer.DocumentNumber, result.Data.DocumentNumber);
            Assert.Equal(customer.Email, result.Data.Email);
        }

        [Fact]
        public async Task ExecuteAsync_ThrowsInvalidOperationException_OnRepositoryException()
        {
            // Arrange
            var documentNumber = "123456789";
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync(documentNumber))
                .ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(documentNumber));
            Assert.Contains("An error occurred while retrieving the customer by document number.", ex.Message);
        }
    }
}
