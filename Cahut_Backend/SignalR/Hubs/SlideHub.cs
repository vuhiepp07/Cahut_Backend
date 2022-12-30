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
        public static Dictionary<string, string> hubConnection = new Dictionary<string, string>();
        private readonly static ConnectionStorage<string> _connections = new ConnectionStorage<string>();
        public readonly static ConnectionStorage<string> _userConnections = new ConnectionStorage<string>();

        public async Task SendResult(string presentationId, string message)
        {
            foreach (var connectionId in _connections.GetConnections(presentationId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveResult", presentationId, message);
            }
            
        }

        public async Task SendMessage(string presentationId, string message)
        {
            AppDbContext context = new AppDbContext();
            ChatRepository chatRepository = new ChatRepository(context);
            List<object> chatMessages = chatRepository.GetChatFromPresentation(Guid.Parse(presentationId));
            Console.WriteLine("Send " + message + " to all " + presentationId);
            foreach (var connectionId in _connections.GetConnections(presentationId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", presentationId, chatMessages.Last());
            }
        }

        public async Task SendQuestion(string presentationId, string question)
        {
            foreach (var connectionId in _connections.GetConnections(presentationId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveQuestion", presentationId, question);
            }
        }

        public async Task ChangeQuestionStatus(string presentationId, string question)
        {
            foreach (var connectionId in _connections.GetConnections(presentationId))
            {
                await Clients.Client(connectionId).SendAsync("ChangeQuestionStatus", presentationId, question);
            }
        }

        public async Task ChangeSlide(string presentationId, string action)
        {
            foreach (var connectionId in _connections.GetConnections(presentationId))
            {
                await Clients.Client(connectionId).SendAsync("ChangeSlide", presentationId, action);
            }
        }

        public async Task NotifyGroupPresent(string groupId)
        {
            foreach (var connectionId in _connections.GetConnections(groupId))
            {
                await Clients.Client(connectionId).SendAsync("NotifyGroup", groupId);
            }
        }

        public override Task OnConnectedAsync()
        {
            //user
            string accessToken = Context.GetHttpContext().Request.Query["access_token"];
            if(accessToken != null)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                string email = handler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "email").Value;
                _userConnections.Add(email, Context.ConnectionId);
            }

            //presentation
            string presentationId = Context.GetHttpContext().Request.Query["presentationId"];
            if(presentationId != null)
            {
                _connections.Add(presentationId, Context.ConnectionId);
                Console.WriteLine("presentationId " + presentationId + " has :" + _connections.GetConnections(presentationId).Count() + " connections");
                
            }
            
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", "I", "disconnect");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
