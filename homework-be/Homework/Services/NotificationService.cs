using Homework.Gateway.Attributes;
using Homework.Gateway.Hubs;
using Homework.Gateway.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Homework.Gateway.Services
{
    [Scoped]
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(IHubContext<NotificationHub> hubContext) 
        {
            _hubContext = hubContext;
        }

        public async Task SendNotification(string message)
        {
            await _hubContext.Clients.All.SendAsync(NotificationHub.RECEIVE_NOTIFICATION, message);
        }

        public async Task SendRequestCount(int count)
        {
            await _hubContext.Clients.All.SendAsync(NotificationHub.RECEIVE_QUEUE_COUNT, count);
        }
    }
}
