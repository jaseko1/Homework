using Homework.Gateway.Data.Models;

namespace Homework.Gateway.Services.Interfaces
{
    public interface IQueueRequestService
    {
        Task<QueueRequest> AddCustomerCreateRequest(OldCustomerService.CreateCustomerRequest request);
        Task<QueueRequest> AddUpdateCustomerRequest(OldCustomerService.UpdateCustomerRequest request);

        Task<QueueRequest> AddDeleteCustomerRequest(OldCustomerService.DeleteCustomerRequest request);

        Task CompleteSuccessfuly(QueueRequest request);

        Task<IEnumerable<QueueRequest>> GetPendingRequests();
    }
}
