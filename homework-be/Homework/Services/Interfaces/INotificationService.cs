namespace Homework.Gateway.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotification(string message);
        Task SendRequestCount(int count);
    }
}
