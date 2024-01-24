using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.OldCustomerService.Models.Requests
{
    public class GetCustomersRequest
    {
        [DataMember(Order = 0)]
        [XmlElement]
        public int PageNumber { get; set; }

        [DataMember(Order = 0)]
        [XmlElement]
        public int PageSize { get; set; }
    }
}
