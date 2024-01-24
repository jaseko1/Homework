using AutoMapper;
using Homework.OldCustomerService.Data.Models;
using Homework.OldCustomerService.Models;
using Homework.OldCustomerService.Models.Requests;

namespace Homework.OldCustomerService.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CreateCustomerRequest>().ReverseMap();
            CreateMap<Customer, UpdateCustomerRequest>().ReverseMap();
            CreateMap<Customer, DeleteCustomerRequest>().ReverseMap();
        }
    }
}
