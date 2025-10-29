using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrintingJobTracker.Infrastructure.Persistence;
using System.Data.Common;

namespace PrintingJobTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SqlConnectionPoolService<ApplicationDbContext>>(serviceProvider =>
            {
                int poolSize = int.Parse(Environment.GetEnvironmentVariable("TAMANO_SQL_POOL") ?? "10");

                string connectionStringCnb = Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION_STRING")
                    ?? "Server=localhost;Database=PrintingJobTracker;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

                return new SqlConnectionPoolService<ApplicationDbContext>(poolSize, connectionStringCnb);
            });

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                SqlConnectionPoolService<ApplicationDbContext> connectionPool = serviceProvider.GetRequiredService<SqlConnectionPoolService<ApplicationDbContext>>();
                DbConnection? connection = connectionPool.GetConnectionAsync().GetAwaiter().GetResult();
                if (connection is not null)
                {
                    options.UseSqlServer(connection);
                }
                else
                {
                    throw new Exception("No se pudo obtener la conexión de base de datos");
                }
            });

            return services;
        }
    }
}
