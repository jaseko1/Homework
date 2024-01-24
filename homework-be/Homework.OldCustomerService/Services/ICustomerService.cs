using Homework.OldCustomerService.Models;
using Homework.OldCustomerService.Models.Requests;
using System.ServiceModel;

namespace Homework.OldCustomerService.Services
{
    [ServiceContract]
    public interface ICustomerService
    {

        [OperationContract]
        Task<CustomerDto> GetOne(string id);

        [OperationContract]
        Task<CustomersDto> Get(GetCustomersRequest request);

        [OperationContract]
        Task Create(CreateCustomerRequest request);

        [OperationContract]
        Task Update(UpdateCustomerRequest request);

        [OperationContract]
        Task Delete(DeleteCustomerRequest request);

    }
}
