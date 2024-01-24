using Homework.Gateway.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Homework.Gateway.Data
{
    public class GatewayDbContext : DbContext
    {
        public GatewayDbContext(DbContextOptions<GatewayDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<QueueRequest> QueueRequests { get; set; }
    }
}