using Microsoft.Extensions.DependencyInjection;
using PrintingJobTracker.Application.Interfaces;
using PrintingJobTracker.Application.Services;

namespace PrintingJobTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IJobService, JobService>();
            services.AddTransient<IClientService, ClientService>();

            return services;
        }
    }
}
