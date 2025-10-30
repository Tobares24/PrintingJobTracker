using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Persistence
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobStatusHistory> JobStatusHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
