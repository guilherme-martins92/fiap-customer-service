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
        private readonly Mock<ISensitiveDataEncryptor> _sensitiveDataEncryptorMock;
        private readonly GetCustomerByIdUseCase _useCase;

        public GetCustomerByIdUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<GetCustomerByIdUseCase>>();
            _sensitiveDataEncryptorMock = new Mock<ISensitiveDataEncryptor>();
            _useCase = new GetCustomerByIdUseCase(
                _customerRepositoryMock.Object,
                _loggerMock.Object,
                _sensitiveDataEncryptorMock.Object
            );
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
        public async Task ExecuteAsync_ShouldReturnFailure_WhenCustomerNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Customer?)null);

            // Act
            var result = await _useCase.ExecuteAsync(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer not found.", result.Errors);  
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnSuccess_WhenCustomerFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customer = new Customer
            {
                Id = id,
                FirstName = "John",
                LastName = "Doe",
                DocumentNumber = "123456789",
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Street = "Main St",
                HouseNumber = "123",
                City = "City",
                State = "State",
                PostalCode = "00000",
                Country = "Country",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(customer);
            _sensitiveDataEncryptorMock.Setup(e => e.DecryptcustomerAsync(customer)).ReturnsAsync(customer);

            // Act
            var result = await _useCase.ExecuteAsync(id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(id, result.Data.Id);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenExceptionThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _useCase.ExecuteAsync(id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("An error occurred while retrieving the customer.", result.Errors);     
        }
    }
}