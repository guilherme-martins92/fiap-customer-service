using Fiap.CustomerService.Application.DTOs;
using Fiap.CustomerService.Application.UseCases.CreateCustomerUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class CreateCustomerUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
        private readonly Mock<IValidator<CustomerInputDto>> _validatorMock = new();
        private readonly Mock<ILogger<CreateCustomerUseCase>> _loggerMock = new();
        private readonly Mock<ISensitiveDataEncryptor> _sensitiveDataEncryptorMock = new();

        private CreateCustomerUseCase CreateUseCase() =>
            new(
                _customerRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object,
                _sensitiveDataEncryptorMock.Object
            );

        private CustomerInputDto GetValidInput() => new()
        {
            FirstName = "John",
            LastName = "Doe",
            DocumentNumber = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890",
            Street = "Main St",
            HouseNumber = "1",
            City = "Metropolis",
            State = "NY",
            PostalCode = "12345",
            Country = "USA"
        };

        private Customer GetCustomerEntity() => new()
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
            City = "Metropolis",
            State = "NY",
            PostalCode = "12345",
            Country = "USA",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenInputIsNull()
        {
            var useCase = CreateUseCase();

            var result = await useCase.ExecuteAsync(null!);

            Assert.False(result.IsSuccess);
            Assert.Contains("Customer input cannot be null.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenValidationFails()
        {
            var useCase = CreateUseCase();
            var input = GetValidInput();
            var errors = new List<ValidationFailure> { new("FirstName", "First name is required.") };
            _validatorMock.Setup(v => v.ValidateAsync(input, default))
                .ReturnsAsync(new ValidationResult(errors));

            var result = await useCase.ExecuteAsync(input);

            Assert.False(result.IsSuccess);
            Assert.Contains("First name is required.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_CreatesCustomerSuccessfully()
        {
            var useCase = CreateUseCase();
            var input = GetValidInput();
            var customerEntity = GetCustomerEntity();

            _validatorMock.Setup(v => v.ValidateAsync(input, default))
                .ReturnsAsync(new ValidationResult());
            _sensitiveDataEncryptorMock.Setup(e => e.EncryptConsumerAsync(It.IsAny<Customer>()))
                .ReturnsAsync(customerEntity);
            _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);
            _sensitiveDataEncryptorMock.Setup(e => e.DecryptcustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync(customerEntity);

            var result = await useCase.ExecuteAsync(input);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(customerEntity.Id, result.Data.Id);
            Assert.Equal(customerEntity.FirstName, result.Data.FirstName);
            Assert.Equal(customerEntity.LastName, result.Data.LastName);
        }
    }
}