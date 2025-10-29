using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Domain.Entities;

namespace PrintingJobTracker.Infrastructure.Persistence
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        SqlConnectionPoolService<ApplicationDbContext> connectionPoolService) : DbContext(options)
    {
        private readonly SqlConnectionPoolService<ApplicationDbContext> _connectionPoolService = connectionPoolService;

        public override void Dispose()
        {
            var dbConn = Database.GetDbConnection();
            if (dbConn is not null)
            {
                _connectionPoolService.ReleaseConnection(dbConn);
            }
            base.Dispose();
        }

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
