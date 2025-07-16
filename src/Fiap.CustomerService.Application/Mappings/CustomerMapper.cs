using Fiap.CustomerService.Application.DTOs;
using Fiap.CustomerService.Domain.Entities;

namespace Fiap.CustomerService.Application.Mappings
{
    public static class CustomerMapper
    {
        public static Customer ToEntity(CustomerInputDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            return new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DocumentNumber = dto.DocumentNumber,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Street = dto.Street,
                HouseNumber = dto.HouseNumber,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static CustomerOutputDto FromEntity(Customer customer)
        {
            if (customer is null) throw new ArgumentNullException(nameof(customer));
            return new CustomerOutputDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DocumentNumber = customer.DocumentNumber,
                DateOfBirth = customer.DateOfBirth,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Street = customer.Street,
                HouseNumber = customer.HouseNumber,
                City = customer.City,
                State = customer.State,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }
    }
}