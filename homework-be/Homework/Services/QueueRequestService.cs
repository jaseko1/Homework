using Homework.Gateway.Attributes;
using Homework.Gateway.Data;
using Homework.Gateway.Data.Models;
using Homework.Gateway.Data.Repositories.Interfaces;
using Homework.Gateway.Services.Interfaces;
using Newtonsoft.Json;

namespace Homework.Gateway.Services
{
    /// <summary>
    /// Service for managing queue requests.
    /// </summary>
    [Scoped]
    public class QueueRequestService : IQueueRequestService
    {
        private readonly IQueueRequestRepository _repository;
        private readonly INotificationService _notificationService;
        public QueueRequestService(
            IQueueRequestRepository repository, 
            INotificationService notificationService
            ) 
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Retrieves all pending queue requests.
        /// </summary>
        /// <returns>A collection of pending queue requests.</returns>
        public async Task<IEnumerable<QueueRequest>> GetPendingRequests()
        {
            return await _repository.GetPendingRequestsAsync();
        }

        /// <summary>
        /// Adds a new create customer request to the queue.
        /// </summary>
        /// <param name="request">The customer creation request.</param>
        /// <returns>The queued request object.</returns>
        public async Task<QueueRequest> AddCustomerCreateRequest(OldCustomerService.CreateCustomerRequest request)
        {
            return await AddQueueRequest(request, RequestMethodTypeEnum.Create);
        }

        /// <summary>
        /// Adds a new update customer request to the queue.
        /// </summary>
        /// <param name="request">The customer update request.</param>
        /// <returns>The queued request object.</returns>
        public async Task<QueueRequest> AddUpdateCustomerRequest(OldCustomerService.UpdateCustomerRequest request)
        {
            return await AddQueueRequest(request, RequestMethodTypeEnum.Update);
        }

        /// <summary>
        /// Adds a new delete customer request to the queue.
        /// </summary>
        /// <param name="request">The customer delete request.</param>
        /// <returns>The queued request object.</returns>
        public async Task<QueueRequest> AddDeleteCustomerRequest(OldCustomerService.DeleteCustomerRequest request)
        {
            return await AddQueueRequest(request, RequestMethodTypeEnum.Delete);
        }

        /// <summary>
        /// Marks a queue request as completed successfully.
        /// </summary>
        ///  <remarks>Request is saved by DbContext Factory - it can be used outside scope</remarks>
        /// <param name="request">The queue request to mark as complete.</param>
        public async Task CompleteSuccessfuly(QueueRequest request)
        {
            request.Status = QueueRequestStatus.Done;
            await _repository.UpdateOutsideScope(request);
            await _notificationService.SendNotification($"[{request.Method}] Request completed successfuly");
        }

        /// <summary>
        /// Marks a queue request as failed.
        /// </summary>
        /// <remarks>Request is saved by DbContext Factory - it can be used outside scope</remarks>
        /// <param name="request">The queue request to mark as failed.</param>
        public async Task CompleteFailed(QueueRequest request)
        {
            request.Status = QueueRequestStatus.Error;
            await _repository.UpdateOutsideScope(request);
            await _notificationService.SendNotification($"[{request.Method}] Request failed");
        }

        /// <summary>
        /// Helper method for adding a queue request.
        /// </summary>
        /// <param name="request">The request object to queue.</param>
        /// <param name="methodType">The type of request method.</param>
        /// <returns>The newly created queue request object.</returns
        private async Task<QueueRequest> AddQueueRequest(object request, RequestMethodTypeEnum methodType)
        {
            var json = JsonConvert.SerializeObject(request);
            var queueRequest = new QueueRequest()
            {
                Method = methodType,
                RequestData = json,
                Status = QueueRequestStatus.Pending
            };

            await _repository.AddQueueRequestAsync(queueRequest);

            return queueRequest;
        }
    }
}
