using Homework.Gateway.Data;
using Homework.Gateway.Hubs;
using Homework.Gateway.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Homework.Gateway.Services
{
    /// <summary>
    /// Handles asynchronous request processing using a concurrent queue.
    /// </summary>
    public class RequestQueueHandler : IRequestQueueHandler
    {
        private readonly ConcurrentQueue<Func<Task>> _requestQueue;
        private readonly SemaphoreSlim _semaphore;
        private bool _isProccessing = false;
        private const int MaxConcurrentRequests = 5;
        private readonly IHubContext<NotificationHub> _hubContext;

        /// <summary>
        /// Initializes a new instance of the RequestQueueHandler class.
        /// </summary>
        /// <param name="hubContext">Hub context for SignalR to send notifications to clients.</param>
        public RequestQueueHandler(IHubContext<NotificationHub> hubContext)
        {
            _requestQueue = new ConcurrentQueue<Func<Task>>();
            _semaphore = new SemaphoreSlim(MaxConcurrentRequests);
            _hubContext = hubContext;
        }

        /// <summary>
        /// Enqueues a request for processing and starts the request processing if not already running.
        /// </summary>
        /// <param name="requestFunc">The asynchronous request function to enqueue.</param>
        public async Task EnqueueRequest(Func<Task> requestFunc)
        {
            _requestQueue.Enqueue(requestFunc);
            await _hubContext.Clients.All.SendAsync(NotificationHub.RECEIVE_QUEUE_COUNT, GetQueueCount());
            if (!_isProccessing)
            {
                await ProcessQueue();
            }
        }

        /// <summary>
        /// Returns the current count of queued requests.
        /// </summary>
        /// <returns>The number of requests in the queue.</returns
        public int GetQueueCount()
        {
            return _requestQueue.Count;
        }

        /// <summary>
        /// Processes the queued requests asynchronously, ensuring a maximum number of concurrent requests.
        /// </summary>
        private async Task ProcessQueue()
        {
            while (_requestQueue.TryDequeue(out var requestAction))
            {
                await _semaphore.WaitAsync();
                try
                {
                    _isProccessing = true;
                    await requestAction();
                }
                finally
                {
                    _semaphore.Release();
                    await _hubContext.Clients.All.SendAsync(NotificationHub.RECEIVE_QUEUE_COUNT, GetQueueCount());
                }
            }
            _isProccessing = false;
        }
    }

}
