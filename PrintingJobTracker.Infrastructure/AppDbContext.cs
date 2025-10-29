using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Infrastructure.Services;

namespace PrintingJobTracker.Infrastructure
{
    public class AppDbContext : DbContext
    {
        private readonly SqlConnectionPoolService<AppDbContext> _connectionPoolService;

        public AppDbContext(SqlConnectionPoolService<AppDbContext> connectionPoolService)
        {
            _connectionPoolService = connectionPoolService;
        }

        public override void Dispose()
        {
            var dbConn = this.Database.GetDbConnection();
            if (dbConn is not null)
            {
                _connectionPoolService.ReleaseConnection(dbConn);
            }
            base.Dispose();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
