using Homework.Gateway.Data.Models;

namespace Homework.Gateway.Data.Repositories.Interfaces
{
    public interface IQueueRequestRepository : IBaseRepository
    {
        Task AddQueueRequestAsync(QueueRequest request);

        Task<IEnumerable<QueueRequest>> GetPendingRequestsAsync();

        Task SaveTransientAsync();

        Task UpdateOutsideScope(QueueRequest request);

    }
}
