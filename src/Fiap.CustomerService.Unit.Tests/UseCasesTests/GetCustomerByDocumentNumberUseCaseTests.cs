using Fiap.CustomerService.Application.UseCases.GetCustomerByDocumentNumberUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class GetCustomerByDocumentNumberUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
        private readonly Mock<ILogger<GetCustomerByDocumentNumberUseCase>> _loggerMock = new();
        private readonly Mock<IHashingService> _hashingServiceMock = new();
        private readonly Mock<ISensitiveDataEncryptor> _sensitiveDataEncryptorMock = new();

        private GetCustomerByDocumentNumberUseCase CreateUseCase()
        {
            return new GetCustomerByDocumentNumberUseCase(
                _customerRepositoryMock.Object,
                _loggerMock.Object,
                _hashingServiceMock.Object,
                _sensitiveDataEncryptorMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenDocumentNumberIsNullOrEmpty()
        {
            var useCase = CreateUseCase();

            var result = await useCase.ExecuteAsync(null!);

            Assert.False(result.IsSuccess);
            Assert.Contains("Document number cannot be null or empty.", result.Errors);

            result = await useCase.ExecuteAsync(string.Empty);

            Assert.False(result.IsSuccess);
            Assert.Contains("Document number cannot be null or empty.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenCustomerNotFound()
        {
            _hashingServiceMock.Setup(h => h.HashValue(It.IsAny<string>())).ReturnsAsync("hashed");
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync("hashed")).ReturnsAsync((Customer?)null);

            var useCase = CreateUseCase();

            var result = await useCase.ExecuteAsync("12345678900");

            Assert.False(result.IsSuccess);
            Assert.Contains("Customer not found.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnSuccess_WhenCustomerFound()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DocumentNumber = "12345678900",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Street = "Main St",
                HouseNumber = "1",
                City = "City",
                State = "ST",
                PostalCode = "12345",
                Country = "Country",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _hashingServiceMock.Setup(h => h.HashValue(It.IsAny<string>())).ReturnsAsync("hashed");
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync("hashed")).ReturnsAsync(customer);
            _sensitiveDataEncryptorMock.Setup(e => e.DecryptcustomerAsync(customer)).ReturnsAsync(customer);

            var useCase = CreateUseCase();

            var result = await useCase.ExecuteAsync("123.456.789-00");

            Assert.True(result.IsSuccess);
            Assert.Equal(customer, result.Data);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowInvalidOperationException_OnException()
        {
            _hashingServiceMock.Setup(h => h.HashValue(It.IsAny<string>())).ThrowsAsync(new Exception("Hash error"));

            var useCase = CreateUseCase();

            await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync("12345678900"));
        }
    }
}
