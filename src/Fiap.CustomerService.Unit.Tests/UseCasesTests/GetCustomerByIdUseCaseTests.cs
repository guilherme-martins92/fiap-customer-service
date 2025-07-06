using Fiap.CustomerService.Application.UseCases.GetCustomerByIdUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class GetCustomerByIdUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<GetCustomerByIdUseCase>> _loggerMock;
        private readonly GetCustomerByIdUseCase _useCase;
        public GetCustomerByIdUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<GetCustomerByIdUseCase>>();
            _useCase = new GetCustomerByIdUseCase(_customerRepositoryMock.Object, _loggerMock.Object);
        }
        [Fact]
        public async Task ExecuteAsync_ShouldReturnSuccess_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer { 
                Id = customerId,
                FirstName = "John",
                LastName = "Doe",
                DocumentNumber = "12345678974",
                DateOfBirth = new DateTime(1985, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                Email = "john.doe@example.com",
                PhoneNumber = "5550000",
                Street = "Main St",
                HouseNumber = "1",
                City = "Metropolis",
                State = "NY",
                PostalCode = "10001",
                Country = "USA",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            var result = await _useCase.ExecuteAsync(customerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync((Customer?)null);
            // Act
            var result = await _useCase.ExecuteAsync(customerId);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer not found.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenIdIsEmpty()
        {
            // Arrange
            var emptyId = Guid.Empty;
            // Act
            var result = await _useCase.ExecuteAsync(emptyId);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid customer ID.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(customerId)).Throws(new Exception("Database error"));
            // Act
            var result = await _useCase.ExecuteAsync(customerId);
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("An error occurred while retrieving the customer.", result.Errors);
        }
    }
}