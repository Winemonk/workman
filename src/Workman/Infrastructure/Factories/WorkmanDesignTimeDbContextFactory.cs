using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using Workman.Infrastructure.DbContexts;

namespace Workman.Infrastructure.Factories
{
    internal class WorkmanDesignTimeDbContextFactory : IDesignTimeDbContextFactory<WorkmanDbContext>
    {
        public WorkmanDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WorkmanDbContext>();
            optionsBuilder.UseSqlite($"data source={Path.Combine(AppContext.BaseDirectory, "../../../wm.db")}");
            return new WorkmanDbContext(optionsBuilder.Options);
        }
    }
}
