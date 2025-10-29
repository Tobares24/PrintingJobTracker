using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Infrastructure.Persistence;

namespace PrintingJobTracker.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task ApplyMigration(this IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                try
                {
                    using (ApplicationDbContext context = serviceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        await context.Database.MigrateAsync();
                    }
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "Error en migración.");
                    throw;
                }
            }
        }
    }
}
