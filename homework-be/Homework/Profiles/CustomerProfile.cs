using AutoMapper;
using Homework.Gateway.API.Models;
using Homework.Gateway.API.Requests;

namespace Homework.Gateway.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile() 
        {       
            CreateMap<OldCustomerService.CreateCustomerRequest, CreateCustomerRequest>().ReverseMap();        
            CreateMap<OldCustomerService.UpdateCustomerRequest, UpdateCustomerRequest>().ReverseMap();                   
            CreateMap<OldCustomerService.GetCustomersRequest, GetCustomersRequest>().ReverseMap();     
            CreateMap<OldCustomerService.CustomersDto, CustomersDto>().ReverseMap();     
            CreateMap<OldCustomerService.CustomerDto, CustomerDto>().ReverseMap();     
        }
    }
}
