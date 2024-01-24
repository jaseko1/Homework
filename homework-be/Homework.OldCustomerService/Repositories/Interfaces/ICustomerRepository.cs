using Homework.OldCustomerService.Data.Models;

namespace Homework.OldCustomerService.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<(IEnumerable<Customer>, int)> GetCustomersAsync(int pageNumber, int pageSize);
        Task<Customer> GetCustomerAsync(string id);
        Task CreateCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(string id);
        // Další metody podle potřeby...
    }
}
