using Microsoft.Extensions.DependencyInjection;

namespace PrintingJobTracker.Infrastructure.Persistence
{
    public class DbContextFactoryService
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactoryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T CreateDbContext<T>() where T : notnull
        {
            return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<T>();
        }
    }
}
