using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.Gateway.API.Requests
{
    public class GetCustomersRequest
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
