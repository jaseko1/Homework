using Homework.Gateway.Data.Models;
using Homework.Gateway.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Homework.Gateway.Data.Repositories
{
    public class QueueRequestRepository : BaseRepository, IQueueRequestRepository
    { 
        private readonly IDbContextFactory<GatewayDbContext> _dbContextFactory; 
        public QueueRequestRepository(GatewayDbContext context, IDbContextFactory<GatewayDbContext> dbContextFactory) : base(context) 
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddQueueRequestAsync(QueueRequest request)
        {
            _context.QueueRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QueueRequest>> GetPendingRequestsAsync()
        {
            return await _context.QueueRequests
                                .Where(r => r.Status == QueueRequestStatus.Pending)
                                .ToListAsync();
        }

        public async Task SaveTransientAsync()
        {
            await using var context = _dbContextFactory.CreateDbContext();
            await context.SaveChangesAsync();
        }

        public async Task UpdateOutsideScope(QueueRequest request)
        {
            await using var context = _dbContextFactory.CreateDbContext();
            context.Update(request);
            await context.SaveChangesAsync();
        }
    }
}
