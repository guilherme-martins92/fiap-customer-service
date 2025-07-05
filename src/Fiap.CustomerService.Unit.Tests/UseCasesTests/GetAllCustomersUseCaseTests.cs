using Fiap.CustomerService.Application.UseCases.GetAllCustomersUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class GetAllCustomersUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<GetAllCustomersUseCase>> _loggerMock;
        private readonly GetAllCustomersUseCase _useCase;

        public GetAllCustomersUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<GetAllCustomersUseCase>>();
            _useCase = new GetAllCustomersUseCase(_customerRepositoryMock.Object, _loggerMock.Object);
        }

        private Customer GetSampleCustomer() => new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            DocumentNumber = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
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

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenCustomersExist()
        {
            // Arrange
            var customers = new List<Customer> { GetSampleCustomer() };
            _customerRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await _useCase.ExecuteAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenNoCustomersFound()
        {
            // Arrange
            _customerRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Customer>());

            // Act
            var result = await _useCase.ExecuteAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Contains("No customers found.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ThrowsInvalidOperationException_WhenRepositoryThrows()
        {
            // Arrange
            _customerRepositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("db error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync());
        }
    }
}