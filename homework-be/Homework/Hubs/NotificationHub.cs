using Microsoft.AspNetCore.SignalR;

namespace Homework.Gateway.Hubs
{
    public class NotificationHub : Hub
    {
        public const string RECEIVE_NOTIFICATION = "ReceiveNotification";
        public const string RECEIVE_QUEUE_COUNT = "ReceiveQueueCount";
    }
}
