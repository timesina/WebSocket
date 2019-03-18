using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebSockets
{
    public class ChatHub:Hub
    {
        public ILogger Logger { get; set; }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("WelCome To World");
        }


        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Logger.LogDebug("A client connected to MyChatHub: " + Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            Logger.LogDebug("A client disconnected from MyChatHub: " + Context.ConnectionId);
        }

        public async Task WelCome(string name)
        {
            await Clients.All.SendAsync("hh");
        }

        public void log(string arg)
        {
            Logger.LogInformation("client send:" + arg);
        }
    }
}