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
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IValidator<CreateCustomerInput>> _validatorMock;
        private readonly Mock<ILogger<CreateCustomerUseCase>> _loggerMock;
        private readonly CreateCustomerUseCase _useCase;

        public CreateCustomerUseCaseTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _validatorMock = new Mock<IValidator<CreateCustomerInput>>();
            _loggerMock = new Mock<ILogger<CreateCustomerUseCase>>();
            _useCase = new CreateCustomerUseCase(
                _customerRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object
            );
        }

        private CreateCustomerInput GetValidInput() => new CreateCustomerInput
        {
            FirstName = "Jane",
            LastName = "Smith",
            DocumentNumber = "987654321",
            DateOfBirth = new DateTime(1985, 5, 20, 0, 0, 0, DateTimeKind.Utc),
            Email = "jane.smith@example.com",
            PhoneNumber = "555-1234",
            Street = "Elm Street",
            HouseNumber = "42",
            City = "Springfield",
            State = "IL",
            PostalCode = "62704",
            Country = "USA"
        };

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenInputIsNull()
        {
            // Act
            var result = await _useCase.ExecuteAsync(null!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer input cannot be null.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var input = GetValidInput();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("FirstName", "First name is required.")
            });
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(validationResult);

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("First name is required.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenDuplicateDocumentNumber()
        {
            // Arrange
            var input = GetValidInput();
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(new ValidationResult());
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync(input.DocumentNumber))
                .ReturnsAsync(new Customer { Id = Guid.NewGuid(), FirstName = "Dup", LastName = "Doc", DocumentNumber = input.DocumentNumber, DateOfBirth = DateTime.Now, Email = "dup@example.com", PhoneNumber = "1", Street = "s", HouseNumber = "1", City = "c", State = "s", PostalCode = "p", Country = "c", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer with this document number already exists.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenDuplicateEmail()
        {
            // Arrange
            var input = GetValidInput();
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(new ValidationResult());
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync(input.DocumentNumber))
                .ReturnsAsync((Customer?)null);
            _customerRepositoryMock.Setup(r => r.GetByEmailAsync(input.Email))
                .ReturnsAsync(new Customer { Id = Guid.NewGuid(), FirstName = "Dup", LastName = "Email", DocumentNumber = "other", DateOfBirth = DateTime.Now, Email = input.Email, PhoneNumber = "1", Street = "s", HouseNumber = "1", City = "c", State = "s", PostalCode = "p", Country = "c", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Customer with this email already exists.", result.Errors);
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsSuccess_WhenInputIsValidAndNoDuplicates()
        {
            // Arrange
            var input = GetValidInput();
            _validatorMock.Setup(v => v.ValidateAsync(input, default)).ReturnsAsync(new ValidationResult());
            _customerRepositoryMock.Setup(r => r.GetByDocumentNumberlAsync(input.DocumentNumber))
                .ReturnsAsync((Customer?)null);
            _customerRepositoryMock.Setup(r => r.GetByEmailAsync(input.Email))
                .ReturnsAsync((Customer?)null);

            Customer? addedCustomer = null;
            _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .Callback<Customer>(c => addedCustomer = c)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(input);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(input.FirstName, result.Data.FirstName);
            Assert.Equal(input.Email, result.Data.Email);
            Assert.NotEqual(Guid.Empty, result.Data.Id);
            Assert.Equal(addedCustomer, result.Data);
        }
    }
}