using Microsoft.AspNetCore.SignalR;
using PrintingJobTracker.Application.Hubs;

namespace PrintingJobTracker.Application.Services
{
    public abstract class BaseService
    {
        protected readonly IHubContext<AppHub> _hubContext;

        public BaseService(IHubContext<AppHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected async Task NotifyAsync<TEntity>(string eventName, TEntity entity)
        {
            await _hubContext.Clients.All.SendAsync(eventName, entity);
        }
    }
}
