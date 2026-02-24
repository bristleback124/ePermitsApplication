using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ePermits.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public async Task JoinApplicationGroup(int applicationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"application-{applicationId}");
        }

        public async Task LeaveApplicationGroup(int applicationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"application-{applicationId}");
        }
    }
}
