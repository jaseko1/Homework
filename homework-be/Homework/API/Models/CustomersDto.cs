namespace Homework.Gateway.API.Models
{
    public class CustomersDto
    {
        public List<CustomerDto> Results { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
