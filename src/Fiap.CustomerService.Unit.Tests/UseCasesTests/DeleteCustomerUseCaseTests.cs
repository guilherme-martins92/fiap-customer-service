using Amazon.DynamoDBv2.Model;
using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Application.UseCases.DeleteCustomerUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class DeleteCustomerUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<DeleteCustomerUseCase>> _loggerMock;
        private readonly DeleteCustomerUseCase _useCase;

        public DeleteCustomerUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<DeleteCustomerUseCase>>();
            _useCase = new DeleteCustomerUseCase(
                _customerRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenIdIsEmpty()
        {
            // Act
            var result = await _useCase.ExecuteAsync(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid customer ID.", result.Errors);
            _customerRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _customerRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenCustomerNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Customer?)null);

            // Act
            var result = await _useCase.ExecuteAsync(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer not found.", result.Errors);
            _customerRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _customerRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenCustomerDeleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customer = new Customer
            {
                Id = id,
                FirstName = "Test",
                LastName = "User",
                DocumentNumber = "123456789",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                Street = "Main St",
                HouseNumber = "1",
                City = "City",
                State = "State",
                PostalCode = "00000",
                Country = "Country",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(customer);
            _customerRepositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            _customerRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _customerRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenExceptionThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customer = new Customer
            {
                Id = id,
                FirstName = "Test",
                LastName = "User",
                DocumentNumber = "123456789",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                Street = "Main St",
                HouseNumber = "1",
                City = "City",
                State = "State",
                PostalCode = "00000",
                Country = "Country",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(customer);
            _customerRepositoryMock.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _useCase.ExecuteAsync(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("An error occurred while deleting the customer.", result.Errors);
            _customerRepositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _customerRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
