using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrintingJobTracker.Infrastructure.Persistence;
using PrintingJobTracker.Infrastructure.Repository.Clients;
using PrintingJobTracker.Infrastructure.Repository.Jobs;
using PrintingJobTracker.Infrastructure.Services;

namespace PrintingJobTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DbContextFactoryService>();
            services.AddSingleton<SeedInitializerService>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IJobStatusHistoryRepository, JobStatusHistoryRepository>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                SqlConnection sqlConnection = new();
                sqlConnection.ConnectionString = Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION_STRING")
                    ?? "Server=localhost;Database=PrintingJobTracker;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
                options.UseSqlServer(sqlConnection);
            });

            return services;
        }
    }
}
