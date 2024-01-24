using Homework.Gateway.API.Models;
using Homework.Gateway.API.Requests;

namespace Homework.Gateway.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomersDto> Get(GetCustomersRequest request);
        Task<CustomerDto> GetOne(string id);
        Task Create(CreateCustomerRequest customer);
        Task Update(UpdateCustomerRequest request);
        Task Delete(string customerId);

        Task FillRequestQueueFromDb();
    }
}
