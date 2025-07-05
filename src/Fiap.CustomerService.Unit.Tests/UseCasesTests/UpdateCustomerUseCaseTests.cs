using Fiap.CustomerService.Application.UseCases.UpdateCustomerUseCase;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.CustomerService.Unit.Tests.UseCasesTests
{
    public class UpdateCustomerUseCaseTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IValidator<UpdateCustomerInput>> _validatorMock;
        private readonly Mock<ILogger<UpdateCustomerUseCase>> _loggerMock;
        private readonly UpdateCustomerUseCase _useCase;

        public UpdateCustomerUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _validatorMock = new Mock<IValidator<UpdateCustomerInput>>();
            _loggerMock = new Mock<ILogger<UpdateCustomerUseCase>>();
            _useCase = new UpdateCustomerUseCase(
                _customerRepositoryMock.Object,
                _loggerMock.Object,
                _validatorMock.Object
            );
        }

        private UpdateCustomerInput GetValidInput() => new UpdateCustomerInput
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890",
            Street = "Main St",
            HouseNumber = "100",
            City = "Metropolis",
            State = "NY",
            PostalCode = "12345",
            Country = "USA"
        };

        private Customer GetExistingCustomer(Guid id) => new Customer
        {
            Id = id,
            FirstName = "Old",
            LastName = "Name",
            DocumentNumber = "DOC123",
            DateOfBirth = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Email = "old.email@example.com",
            PhoneNumber = "0000000000",
            Street = "Old St",
            HouseNumber = "1",
            City = "Old City",
            State = "OS",
            PostalCode = "00000",
            Country = "OldCountry",
            CreatedAt = DateTime.UtcNow.AddYears(-1),
            UpdatedAt = DateTime.UtcNow.AddMonths(-1)
        };

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var input = GetValidInput();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("FirstName", "First name is required.")
            });
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _useCase.ExecuteAsync(id, input);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("First name is required.", result.Errors);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Validation failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenCustomerNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var input = GetValidInput();
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(new ValidationResult());
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Customer?)null);

            // Act
            var result = await _useCase.ExecuteAsync(id, input);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer not found", result.Errors);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenUpdateIsSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var input = GetValidInput();
            var existingCustomer = GetExistingCustomer(id);
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(new ValidationResult());
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingCustomer);
            _customerRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(id, input);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(input.FirstName, result.Data.FirstName);
            Assert.Equal(input.LastName, result.Data.LastName);
            Assert.Equal(input.Email, result.Data.Email);
            Assert.Equal(input.PhoneNumber, result.Data.PhoneNumber);
            Assert.Equal(input.Street, result.Data.Street);
            Assert.Equal(input.HouseNumber, result.Data.HouseNumber);
            Assert.Equal(input.City, result.Data.City);
            Assert.Equal(input.State, result.Data.State);
            Assert.Equal(input.PostalCode, result.Data.PostalCode);
            Assert.Equal(input.Country, result.Data.Country);
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Customer>(c => c.Id == id)), Times.Once);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully updated")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenExceptionIsThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            var input = GetValidInput();
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(new ValidationResult());
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _useCase.ExecuteAsync(id, input);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("An unexpected error occurred", result.Errors);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}