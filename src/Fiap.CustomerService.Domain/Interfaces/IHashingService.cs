namespace Fiap.CustomerService.Domain.Interfaces
{
    public interface IHashingService
    {
        Task<string> HashValue(string value);
    }
}