using Fiap.CustomerService.Application.UseCases.GetCustomerByEmailUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class GetCustomerByEmailUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<GetCustomerByEmailUseCase>> _loggerMock;
        private readonly GetCustomerByEmailUseCase _useCase;

        public GetCustomerByEmailUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<GetCustomerByEmailUseCase>>();
            _useCase = new GetCustomerByEmailUseCase(
                _customerRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenCustomerFound()
        {
            // Arrange
            var email = "teste@email.com";
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DocumentNumber = email,
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

            _customerRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(customer);
            // Act
            var result = await _useCase.ExecuteAsync(email);
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(customer, result.Data);
        }
        
        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenEmailIsNullOrEmpty()
        {
            // Act
            var resultNull = await _useCase.ExecuteAsync(null!);
            var resultEmpty = await _useCase.ExecuteAsync("");
            var resultWhitespace = await _useCase.ExecuteAsync("   ");
            // Assert
            Assert.False(resultNull.IsSuccess);
            Assert.Contains("Email is required.", resultNull.Errors);
            Assert.False(resultEmpty.IsSuccess);
            Assert.Contains("Email is required.", resultEmpty.Errors);
            Assert.False(resultWhitespace.IsSuccess);
            Assert.Contains("Email is required.", resultWhitespace.Errors);
        }
        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenCustomerNotFound()
        {
            // Arrange
            var email = "";
            _customerRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((Customer?)null);
            // Act
            var result = await _useCase.ExecuteAsync(email);
            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task ExecuteAsync_ThrowsInvalidOperationException_OnRepositoryException()
        {
            // Arrange
            var email = "teste@email.com";
            _customerRepositoryMock.Setup(r => r.GetByEmailAsync(email))
                .ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(email));
            Assert.Contains("An error occurred while retrieving the customer by email.", ex.Message);
        }
    }
}