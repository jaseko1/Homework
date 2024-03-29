﻿using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Homework.Gateway.API.Models
{
    public class CustomerDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
