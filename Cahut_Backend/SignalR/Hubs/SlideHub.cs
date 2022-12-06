using Microsoft.AspNetCore.SignalR;

namespace Cahut_Backend.SignalR.Hubs
{
    public class SlideHub: Hub
    {
        public async Task SendResult(string channel, string message)
        {
            Console.WriteLine(channel + " " + message);
            await Clients.All.SendAsync(channel, message);
        }
    }
}
