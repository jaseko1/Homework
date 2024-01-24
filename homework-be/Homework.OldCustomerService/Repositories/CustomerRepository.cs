using Homework.OldCustomerService.Attributes;
using Homework.OldCustomerService.Data;
using Homework.OldCustomerService.Data.Models;
using Homework.OldCustomerService.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Homework.OldCustomerService.Repositories
{
    [Scoped]
    public class CustomerRepository : ICustomerRepository
    {
        private readonly OldServiceDbContext _context;

        public CustomerRepository(OldServiceDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Customer>, int)> GetCustomersAsync(int pageNumber, int pageSize)
        {
            var query = _context.Customers.AsQueryable();

            var totalRecords = await query.CountAsync();
            var customers = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (customers, totalRecords);
        }

        public async Task<Customer> GetCustomerAsync(string id)
        {
            return await _context.Customers.AsQueryable()
                .Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            await _context.Customers.InsertOneAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _context.Customers.ReplaceOneAsync(c => c.Id == customer.Id, customer);
        }

        public async Task DeleteCustomerAsync(string id)
        {
            await _context.Customers.DeleteOneAsync(c => c.Id == id);
        }
        // Implementace dalších metod...
    }
}
