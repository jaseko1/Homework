﻿using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.OldCustomerService.Models.Requests
{
    public class DeleteCustomerRequest
    {
        [DataMember]
        [XmlElement]
        public string Id { get; set; }
    }
}
