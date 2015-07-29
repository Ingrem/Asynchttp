using Microsoft.AspNet.SignalR;

namespace WebService.Controllers
{
    public class LongPolling : Hub
    {
        public void Send(string message)
        {
            Clients.Client(Context.ConnectionId).addMessage(message);
        }
    }
}