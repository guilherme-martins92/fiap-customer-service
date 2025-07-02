namespace Fiap.CustomerService.Application.UseCases.CreateCustomerUseCase
{
    public class CreateCustomerInput
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string DocumentNumber { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}