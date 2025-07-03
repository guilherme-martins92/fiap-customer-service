using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;

namespace Fiap.CustomerService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers = new List<Customer>();
        public Task AddAsync(Customer customer)
        {
            customer.Id = _customers.Count + 1; // Simple ID generation
            _customers.Add(customer);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Customer>>(_customers);
        }

        public Task<Customer?> GetByIdAsync(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(customer);
        }
        public Task UpdateAsync(Customer customer)
        {
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.DocumentNumber = customer.DocumentNumber;
                existingCustomer.DateOfBirth = customer.DateOfBirth;
                existingCustomer.Email = customer.Email;
                existingCustomer.PhoneNumber = customer.PhoneNumber;
                existingCustomer.Street = customer.Street;
                existingCustomer.HouseNumber = customer.HouseNumber;
                existingCustomer.City = customer.City;
                existingCustomer.State = customer.State;
                existingCustomer.PostalCode = customer.PostalCode;
                existingCustomer.Country = customer.Country;
                existingCustomer.UpdatedAt = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }
        public Task DeleteAsync(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
            return Task.CompletedTask;
        }
    }
}