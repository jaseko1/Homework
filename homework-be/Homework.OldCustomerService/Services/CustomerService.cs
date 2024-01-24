using AutoMapper;
using Homework.OldCustomerService.Attributes;
using Homework.OldCustomerService.Data;
using Homework.OldCustomerService.Data.Models;
using Homework.OldCustomerService.Models;
using Homework.OldCustomerService.Models.Requests;
using Homework.OldCustomerService.Repositories.Interfaces;
using Homework.OldCustomerService.Services.Interfaces;

namespace Homework.OldCustomerService.Services
{
    [Scoped]
    public class CustomerService : ICustomerService
    {
        private readonly IRequestCounter _requestCounter;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(
            OldServiceDbContext context,
            IRequestCounter requestCounter,
            ICustomerRepository customerRepository,
            IMapper mapper
            )
        {
            _requestCounter = requestCounter;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<CustomersDto> Get(GetCustomersRequest request)
        {
            var customerList = await _customerRepository.GetCustomersAsync(request.PageNumber, request.PageSize);
            var customerDtos = _mapper.Map<List<CustomerDto>>(customerList.Item1);

            return new CustomersDto()
            {
                TotalRecords = customerList.Item2,
                Results = customerDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<CustomerDto> GetOne(string id)
        {
            var customer = await _customerRepository.GetCustomerAsync(id);
            var dto = _mapper.Map<CustomerDto>(customer);
            return dto;
        }

        public async Task Update(UpdateCustomerRequest request)
        {
            _requestCounter.Increment();
            try
            {
                ProcessRequest();
                var customer = _mapper.Map<Customer>(request);
                await _customerRepository.UpdateCustomerAsync(customer);
            }
            finally
            {
                _requestCounter.Decrement();
            }
        }

        public async Task Create(CreateCustomerRequest request)
        {
            _requestCounter.Increment();
            try
            {
                ProcessRequest();
                var customer = _mapper.Map<Customer>(request);
                await _customerRepository.CreateCustomerAsync(customer);
            }
            finally
            {
                _requestCounter.Decrement();
            }
        }

        public async Task Delete(DeleteCustomerRequest request)
        {
            _requestCounter.Increment();
            try
            {
                ProcessRequest();
                await _customerRepository.DeleteCustomerAsync(request.Id);
            }
            finally
            {
                _requestCounter.Decrement();
            }
        }

        private void ProcessRequest()
        {
            var random = new Random();
            var number = random.Next(0, 10);
            Thread.Sleep(number * 1000);
        }
    }
}
