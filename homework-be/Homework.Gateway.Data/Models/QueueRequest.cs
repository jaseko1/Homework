using Homework.Gateway.Data.Models.Shared;

namespace Homework.Gateway.Data.Models
{
    public class QueueRequest : DateEntity
    {
        public QueueRequestStatus Status { get; set; }

        public RequestMethodTypeEnum Method { get; set; }

        public string? RequestData { get; set; }
    }
}
