using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebServer.HubControllers
{
    public class Lobby : Hub
    {
		public override async Task OnConnectedAsync()
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
			await base.OnDisconnectedAsync(exception);
		}

        public async Task SendMessage(string user, string message)
    	{
        	await Clients.All.SendAsync("ReceiveMessage", user, message);
    	}
    }
}
