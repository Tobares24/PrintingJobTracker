using Microsoft.AspNetCore.SignalR;

namespace PrintingJobTracker.Application.Hubs
{
    public class AppHub : Hub
    {
        public async Task SendNotification(string eventName, object data)
        {
            await Clients.All.SendAsync(eventName, data);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
