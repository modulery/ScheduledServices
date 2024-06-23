using Microsoft.EntityFrameworkCore;

namespace Moduler.ScheduledServices.Data
{
    public class HangfireDbContext : DbContext
    {
        public HangfireDbContext(DbContextOptions<HangfireDbContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ProjectEnvironment> ProjectEnvironments { get; set; }
        //public DbSet<HangfireJob> HangfireJobs { get; set; }
    }
}
