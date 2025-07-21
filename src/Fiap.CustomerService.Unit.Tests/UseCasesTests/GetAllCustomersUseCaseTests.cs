using Fiap.CustomerService.Application.UseCases.GetAllCustomersUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class GetAllCustomersUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<GetAllCustomersUseCase>> _loggerMock;
        private readonly Mock<ISensitiveDataEncryptor> _sensitiveDataEncryptorMock;
        private readonly GetAllCustomersUseCase _useCase;

        public GetAllCustomersUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<GetAllCustomersUseCase>>();
            _sensitiveDataEncryptorMock = new Mock<ISensitiveDataEncryptor>();
            _useCase = new GetAllCustomersUseCase(
                _customerRepositoryMock.Object,
                _loggerMock.Object,
                _sensitiveDataEncryptorMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenNoCustomersFound()
        {
            _customerRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Customer>());

            var result = await _useCase.ExecuteAsync();

            Assert.False(result.IsSuccess);
            Assert.Contains("No customers found.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WithMappedCustomers()
        {
            var customers = new List<Customer>
        {
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DocumentNumber = "123456789",
                DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Street = "Main St",
                HouseNumber = "1",
                City = "City",
                State = "State",
                PostalCode = "00000",
                Country = "Country",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

            _customerRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(customers);

            _sensitiveDataEncryptorMock.Setup(e => e.DecryptCustomersAsync(It.IsAny<IEnumerable<Customer>>()))
                .ReturnsAsync(customers);

            var result = await _useCase.ExecuteAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            var dto = result.Data.First();
            Assert.Equal(customers[0].Id, dto.Id);
            Assert.Equal(customers[0].FirstName, dto.FirstName);
            Assert.Equal(customers[0].LastName, dto.LastName);
        }

        [Fact]
        public async Task ExecuteAsync_SkipsNullCustomers()
        {
            var customers = new List<Customer?>
        {
            null,
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                DocumentNumber = "987654321",
                DateOfBirth = new DateTime(1985, 5, 5, 0, 0, 0, DateTimeKind.Utc),
                Email = "jane.smith@example.com",
                PhoneNumber = "0987654321",
                Street = "Second St",
                HouseNumber = "2",
                City = "Town",
                State = "Province",
                PostalCode = "11111",
                Country = "Country",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

            _customerRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(customers.Where(c => c != null)! /* filter out nulls */.Cast<Customer>());

            _sensitiveDataEncryptorMock.Setup(e => e.DecryptCustomersAsync(It.IsAny<IEnumerable<Customer>>()))
                .ReturnsAsync(customers.Where(c => c != null)!.Cast<Customer>());

            var result = await _useCase.ExecuteAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data); // Ensure result.Data is not null before using it
            Assert.Single(result.Data);
            Assert.Equal("Jane", result.Data.First().FirstName);
        }

        [Fact]
        public async Task ExecuteAsync_ThrowsInvalidOperationException_OnException()
        {
            _customerRepositoryMock.Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync());
        }
    }
}
