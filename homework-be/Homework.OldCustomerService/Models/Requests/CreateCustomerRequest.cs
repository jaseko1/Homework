using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.OldCustomerService.Models.Requests
{
    public class CreateCustomerRequest
    {

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
