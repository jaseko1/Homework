using Homework.Gateway.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Gateway.Data.Repositories
{
    public class BaseRepository
    {
        protected readonly GatewayDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public BaseRepository(GatewayDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
