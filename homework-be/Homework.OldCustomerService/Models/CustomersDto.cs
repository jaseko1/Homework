using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.OldCustomerService.Models
{
    [DataContract]
    public class CustomersDto
    {
        [DataMember]
        public List<CustomerDto> Results { get; set; }

        [DataMember]
        [XmlElement]
        public int TotalRecords { get; set; }

        [DataMember]
        [XmlElement]
        public int PageSize { get; set; }

        [DataMember]
        [XmlElement]
        public int PageNumber { get; set; }
    }
}
