namespace Homework.Gateway.Services.Interfaces
{
    public interface IRequestQueueHandler
    {
        Task EnqueueRequest(Func<Task> requestFunc);
        int GetQueueCount();
    }
}
