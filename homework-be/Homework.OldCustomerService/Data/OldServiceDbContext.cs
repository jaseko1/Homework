using Homework.OldCustomerService.Data.Models;
using MongoDB.Driver;

namespace Homework.OldCustomerService.Data
{
    public class OldServiceDbContext
    {
        private readonly IMongoDatabase _database;

        public OldServiceDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("OldCustomerServiceDb");
        }

        public IMongoCollection<Customer> Customers => _database.GetCollection<Customer>("Customers");
    }
}
