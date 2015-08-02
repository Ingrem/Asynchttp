using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebService.Service
{
    public class LongPolling : Hub
    {
        readonly RabbitQueue _rabbit = new RabbitQueue();
        static readonly Dictionary<string, string> GetId = new Dictionary<string, string>();
        public void Send(string deviceId)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<LongPolling>();
            string message = _rabbit.Consumer(deviceId);
            if (message != null)
            {
                context.Clients.Client(GetId[deviceId]).addNewMessageToPage(message);
                GetId.Remove(deviceId);
            }
        }

        public void AskAll(string deviceId)
        {
            Send(deviceId);
        }

        public void SetId(string deviceId)
        {
            GetId.Add(deviceId, Context.ConnectionId);
        }
    }             
}