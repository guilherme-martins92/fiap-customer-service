using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;

namespace Fiap.CustomerService.Application.UseCases.GetCustomerByIdUseCase
{
    public class GetCustomerByIdOutput
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string HouseNumber { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public string Country { get; set; } = default!;

        public static GetCustomerByIdOutput FromEntity(Customer customer)
        {
            return new GetCustomerByIdOutput
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DocumentNumber = FormatUtils.FormatDocumentNumber(customer.DocumentNumber),
                DateOfBirth = customer.DateOfBirth,
                Email = customer.Email,
                PhoneNumber = FormatUtils.FormatPhoneNumber(customer.PhoneNumber),
                Street = customer.Street,
                HouseNumber = customer.HouseNumber,
                City = customer.City,
                State = customer.State,
                PostalCode = FormatUtils.FormatPostalCode(customer.PostalCode),
                Country = customer.Country
            };
        }
    }
}