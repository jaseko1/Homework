using AutoMapper;
using Homework.Gateway.API.Requests;
using Homework.Gateway.Data.Models;
using Homework.Gateway.Services;
using Homework.Gateway.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Homework.Gateway.Tests
{
    public class CustomerServicesTests
    {
        [Fact]
        public async Task CreateCustomer_EnqueuesRequestAndHandlesItProperly()
        {
            // Arrange
            var mockRequestQueueHandler = new Mock<IRequestQueueHandler>();
            var mockOldServiceFactory = new Mock<OldCustomerService.IOldCustomerServiceFactory>();
            var mockOldService = new Mock<OldCustomerService.ICustomerService>();
            var mockLogger = new Mock<ILogger<CustomerService>>();
            var mockMapper = new Mock<IMapper>();
            var mockQueueRequestService = new Mock<IQueueRequestService>();

            var customerService = new CustomerService(
                mockRequestQueueHandler.Object, mockOldServiceFactory.Object,
                mockLogger.Object, mockMapper.Object, mockQueueRequestService.Object);

            var customerRequest = new CreateCustomerRequest();
            var oldServiceCreateRequest = new OldCustomerService.CreateCustomerRequest();

            mockMapper.Setup(m => m.Map<OldCustomerService.CreateCustomerRequest>(It.IsAny<CreateCustomerRequest>()))
                      .Returns(oldServiceCreateRequest);

            // Simulujeme pøidání požadavku do fronty
            mockQueueRequestService.Setup(m => m.AddCustomerCreateRequest(It.IsAny<OldCustomerService.CreateCustomerRequest>()))
                                   .ReturnsAsync(new QueueRequest());

            // Act
            await customerService.Create(customerRequest);

            // Assert
            mockQueueRequestService.Verify(m => m.AddCustomerCreateRequest(It.Is<OldCustomerService.CreateCustomerRequest>(req => req == oldServiceCreateRequest)), Times.Once);
            mockRequestQueueHandler.Verify(m => m.EnqueueRequest(It.IsAny<Func<Task>>()), Times.Once);
        }

    }
}