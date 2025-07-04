using Fiap.CustomerService.Application.Common;
using Fiap.CustomerService.Domain.Entities;
using Fiap.CustomerService.Domain.Interfaces;

namespace Fiap.CustomerService.Application.UseCases.GetCustomerByEmailUseCase
{
    public class GetCustomerByEmailUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        public GetCustomerByEmailUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<Result<Customer>> ExecuteAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result<Customer>.Failure(new List<string> { "Email is required." });
            }
            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return Result<Customer>.Failure(new List<string> { "Customer not found." });
            }
            return Result<Customer>.Success(customer);
        }
    }
}