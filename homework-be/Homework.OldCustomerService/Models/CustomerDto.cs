using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.OldCustomerService.Models
{
    [DataContract]
    public class CustomerDto
    {
        [DataMember]
        [XmlElement]
        public string Id { get; set; }

        [DataMember]
        [XmlElement]
        public string FirstName { get; set; }

        [DataMember]
        [XmlElement]
        public string LastName { get; set; }

        [DataMember]
        [XmlElement]
        public string Email { get; set; }

        [DataMember]
        [XmlElement]
        public string Phone { get; set; }
    }
}
