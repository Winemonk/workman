using Microsoft.EntityFrameworkCore;
using Workman.Core.Entities;

namespace Workman.Infrastructure.DbContexts
{
    internal class WorkmanDbContext : DbContext
    {
        public WorkmanDbContext(DbContextOptions<WorkmanDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkLog>()
                        .HasOne<WorkTask>()
                        .WithMany()
                        .HasForeignKey(x => x.TaskId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WorkTask>()
                        .HasOne<WorkProject>()
                        .WithMany()
                        .HasForeignKey(x => x.ProjectId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
