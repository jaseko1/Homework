using Microsoft.EntityFrameworkCore;

namespace Homework.Hangfire
{
    public class HangfireDbContext : DbContext
    {
        public HangfireDbContext(
            DbContextOptions<HangfireDbContext> options)
            : base(options)
        { }
    }
}
