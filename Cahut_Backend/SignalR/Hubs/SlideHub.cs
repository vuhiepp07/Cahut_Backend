using Cahut_Backend.Models;
using Cahut_Backend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cahut_Backend.SignalR.Hubs
{
    public class SlideHub: Hub
    {
        Dictionary<string, string> hubConnection = new Dictionary<string, string>();
        private readonly static ConnectionStorage<string> _connections =
            new ConnectionStorage<string>();
        public async Task SendResult(string slideId, string message)
        {
            Console.WriteLine(slideId + " " + message);
            foreach (var connectionId in _connections.GetConnections(slideId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveResult", slideId, message);
            }
            
        }

        public async Task SendMessage(string userId, string message)
        {

        }

        public override Task OnConnectedAsync()
        {
            string slideId = Context.GetHttpContext().Request.Query["slideId"];
            Console.WriteLine(slideId);
            if(slideId != null)
            {
                _connections.Add(slideId, Context.ConnectionId);
            }
            Console.WriteLine(_connections.Count);
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", "I", "disconnect");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
