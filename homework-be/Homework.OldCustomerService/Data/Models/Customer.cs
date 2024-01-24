using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Homework.OldCustomerService.Data.Models
{
    public class Customer
    {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("FirstName")]
    public string FirstName { get; set; }

    [BsonElement("LastName")]
    public string LastName { get; set; }

    [BsonElement("Email")]
    public string Email { get; set; }

    [BsonElement("Phone")]
    public string Phone { get; set; }

    }
}
