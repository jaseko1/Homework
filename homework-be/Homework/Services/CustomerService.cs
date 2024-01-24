using AutoMapper;
using Homework.Gateway.Data.Models;
using Homework.Gateway.Services.Interfaces;
using Homework.Gateway.Data;
using Newtonsoft.Json;
using Homework.Gateway.Attributes;
using Homework.Gateway.API.Models;
using Homework.Gateway.API.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Gateway.Services
{
    [Scoped]
    public class CustomerService : ICustomerService
    {
        private readonly IRequestQueueHandler _requestQueueManager;
        private readonly OldCustomerService.ICustomerService _oldService;
        private readonly ILogger<CustomerService> _logger;
        private readonly IMapper _mapper;
        private readonly IQueueRequestService _queueRequestService;

        public CustomerService(
            IRequestQueueHandler requestQueueManager,
            OldCustomerService.IOldCustomerServiceFactory oldCustomerServiceFactory,
            ILogger<CustomerService> logger,
            IMapper mapper,
            IQueueRequestService queueRequestService)
        {
            _requestQueueManager = requestQueueManager;
            _oldService = oldCustomerServiceFactory.CreateService();
            _logger = logger;
            _mapper = mapper;
            _queueRequestService = queueRequestService;
        }

        /// <summary>
        /// Retrieves a list of customers based on the provided request criteria.
        /// </summary>
        /// <param name="request">Request containing the search criteria.</param>
        /// <returns>A DTO containing the list of customers.</returns>
        /// <exception cref="Exception">Thrown when an error occurs in the process.</exception>
        public async Task<CustomersDto> Get(GetCustomersRequest request)
        {
            try
            {
                var requestSoap = _mapper.Map<OldCustomerService.GetCustomersRequest>(request);
                var customerSoapDto = await _oldService.GetAsync(new OldCustomerService.GetRequest()
                {
                    Body = new OldCustomerService.GetRequestBody()
                    {
                        request = requestSoap
                    }
                });
                var dto = _mapper.Map<CustomersDto>(customerSoapDto.Body.GetResult);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching customers");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a single customer by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the customer.</param>
        /// <returns>A DTO representing the customer.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no customer is found.</exception>
        /// <exception cref="Exception">Thrown when an error occurs in the process.</exception>
        public async Task<CustomerDto> GetOne(string id)
        {
            try
            {
                
                var customerSoapDto = await _oldService.GetOneAsync(new OldCustomerService.GetOneRequest()
                {
                    Body = new OldCustomerService.GetOneRequestBody()
                    {
                        id = id
                    }
                }) ?? throw new KeyNotFoundException($"");
                var dto = _mapper.Map<CustomerDto>(customerSoapDto.Body.GetOneResult);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching customer with ID {id}.");
                throw;
            }
        }

        /// <summary>
        /// Creates a new customer based on the provided customer data.
        /// </summary>
        /// <param name="customer">The customer data for creating a new record.</param>
        /// <remarks>Creates are queued and processed asynchronously.</remarks>
        /// <exception cref="Exception">Thrown when an error occurs during the creation process.</exception>
        public async Task Create(CreateCustomerRequest customer)
        {
            try
            {
                var request = _mapper.Map<OldCustomerService.CreateCustomerRequest>(customer);
                var queueRequest = await _queueRequestService.AddCustomerCreateRequest(request);
                await _requestQueueManager.EnqueueRequest(() => CreateCustomerAsync(request, queueRequest));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating customer.");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing customer's information.
        /// </summary>
        /// <param name="request">Data required for updating the customer.</param>
        /// <remarks>Updates are queued and processed asynchronously.</remarks>
        public async Task Update(UpdateCustomerRequest request)
        {
            var requestSoap = _mapper.Map<OldCustomerService.UpdateCustomerRequest>(request);
            var queueRequest = await _queueRequestService.AddUpdateCustomerRequest(requestSoap);
            await _requestQueueManager.EnqueueRequest(() => UpdateCustomerAsync(requestSoap, queueRequest));
        }

        /// <summary>
        /// Deletes a customer based on their unique identifier.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer to be deleted.</param>
        /// <remarks>Deletions are queued and processed asynchronously.</remarks>
        public async Task Delete(string customerId)
        {
            var requestSoap = new OldCustomerService.DeleteCustomerRequest()
            {
                Id = customerId
            };
            var queueRequest = await _queueRequestService.AddDeleteCustomerRequest(requestSoap);
            await _requestQueueManager.EnqueueRequest(() => DeleteCustomerAsync(requestSoap, queueRequest));
        }


        /// <summary>
        /// Processes pending customer requests stored in the database.
        /// </summary>
        /// <remarks>
        /// This method re-enqueues any pending requests for create, update, or delete operations.
        /// It ensures the system's resiliency to interruptions by persisting requests.
        /// </remarks>
        public async Task FillRequestQueueFromDb()
        {
            var pendingRequests = await _queueRequestService.GetPendingRequests();
            foreach (var pendingRequest in pendingRequests)
            {
                if (pendingRequest.RequestData == null)
                    continue;

                switch (pendingRequest.Method)
                {
                    case RequestMethodTypeEnum.Create:
                        var createRequest = JsonConvert.DeserializeObject<OldCustomerService.CreateCustomerRequest>(pendingRequest.RequestData);
                        await _requestQueueManager.EnqueueRequest(() => CreateCustomerAsync(createRequest, pendingRequest));
                        break;
                    case RequestMethodTypeEnum.Update:
                        var updateRequest = JsonConvert.DeserializeObject<OldCustomerService.UpdateCustomerRequest>(pendingRequest.RequestData);
                        await _requestQueueManager.EnqueueRequest(() => UpdateCustomerAsync(updateRequest, pendingRequest));
                        break;
                    case RequestMethodTypeEnum.Delete:
                        var deleteRequest = JsonConvert.DeserializeObject<OldCustomerService.DeleteCustomerRequest>(pendingRequest.RequestData);
                        await _requestQueueManager.EnqueueRequest(() => DeleteCustomerAsync(deleteRequest, pendingRequest));
                        break;
                }
            }
        }

        /// <summary>
        /// Asynchronously creates a customer in the old service.
        /// </summary>
        /// <param name="request">The customer creation request.</param>
        /// <param name="queueRequest">The queued request for tracking and management.</param>
        /// <exception cref="Exception">Thrown when an error occurs during the creation process.</exception>
        private async Task CreateCustomerAsync(OldCustomerService.CreateCustomerRequest request, QueueRequest queueRequest)
        {
            try
            {
                await _oldService.CreateAsync(new OldCustomerService.CreateRequest()
                {
                    Body = new OldCustomerService.CreateRequestBody()
                    {
                        request = request
                    }
                });
                await _queueRequestService.CompleteSuccessfuly(queueRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating customer-");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously updates a customer's information in the old service.
        /// </summary>
        /// <param name="request">The update request data.</param>
        /// <param name="queueRequest">The queued request for tracking and management.</param>
        /// <exception cref="Exception">Thrown when an error occurs during the update process.</exception>
        private async Task UpdateCustomerAsync(OldCustomerService.UpdateCustomerRequest request, QueueRequest queueRequest)
        {
            try
            {
                await _oldService.UpdateAsync(new OldCustomerService.UpdateRequest()
                {
                    Body = new OldCustomerService.UpdateRequestBody()
                    {
                        request = request
                    }
                });
                await _queueRequestService.CompleteSuccessfuly(queueRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching customer with ID {request.Id}.");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously deletes a customer in the old service.
        /// </summary>
        /// <param name="request">The delete request data.</param>
        /// <param name="queueRequest">The queued request for tracking and management.</param>
        /// <exception cref="Exception">Thrown when an error occurs during the delete process.</exception>
        private async Task DeleteCustomerAsync(OldCustomerService.DeleteCustomerRequest request, QueueRequest queueRequest)
        {
            try
            {
                await _oldService.DeleteAsync(new OldCustomerService.DeleteRequest()
                {
                    Body = new OldCustomerService.DeleteRequestBody()
                    {
                        request = request
                    }
                });
                await _queueRequestService.CompleteSuccessfuly(queueRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching customer with ID {request.Id}.");
                throw;
            }
        }

    }
}
