using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.OldCustomerService.Models
{
    public class PaginatingDto<T> where T : class
    {

        [DataMember]
        [XmlElement]
        public IEnumerable<T> Data { get; set; }

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
