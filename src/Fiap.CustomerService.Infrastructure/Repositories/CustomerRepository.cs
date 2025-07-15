using Amazon.DynamoDBv2.DataModel;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;

namespace Fiap.CustomerService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    { 
        private readonly IDynamoDBContext _context;

        public CustomerRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.SaveAsync(customer);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = await _context.ScanAsync<Customer>(new List<ScanCondition>()).GetRemainingAsync();
            return customers;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            var customer = await _context.LoadAsync<Customer>(id);
            return customer;
        }

        public async Task<Customer?> GetByDocumentNumberlAsync(string documentNumber)
        {
            var queryConfig = new QueryConfig
            {
                IndexName = "DocumentNumberHash-index"
            };

            var search = _context.QueryAsync<Customer>(documentNumber, queryConfig);
            var results = await search.GetNextSetAsync();
            return results.FirstOrDefault();
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            var queryConfig = new QueryConfig
            {
                IndexName = "EmailHash-index"
            };

            var search = _context.QueryAsync<Customer>(email, queryConfig);
            var results = await search.GetNextSetAsync();
            return results.FirstOrDefault();
        }

        public async Task UpdateAsync(Customer customer)
        {
            var existingCustomer = await _context.LoadAsync<Customer>(customer.Id);
            if (existingCustomer != null)
            {
                await _context.SaveAsync(customer);
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            var customer = await _context.LoadAsync<Customer>(id);
            if (customer != null)
            {
                await _context.DeleteAsync(customer);
            }
        }
    }
}