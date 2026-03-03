using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Workman.Core.Repositories;
using Workman.Infrastructure.DbContexts;
using Workman.Infrastructure.Repositories;

namespace GeoAppPackager.Infrastructure
{
    public static class InfrastructureSetup
    {
        public static ServiceCollection AddInfrastructure(this ServiceCollection services)
        {
            SQLitePCL.Batteries.Init();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddDbContext<WorkmanDbContext>(
                options => options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "wm.db")}"),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton);
            return services;
        }
    }
}
