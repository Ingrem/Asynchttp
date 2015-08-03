using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace WebService.Service
{
    public class LongPolling
    {
        public static Dictionary<string, string> JsonStrings = new Dictionary<string, string>();
        public static Collection<String> Connections = new Collection<string>();

        public string CommandWaitPolling(string deviceId, int timeout)
        {
            RabbitQueue rabbit = new RabbitQueue();
            TimerCallback tm = Count;

            JsonStrings.Add(deviceId, rabbit.Consumer(deviceId));
            Connections.Add(deviceId);
            var timer = new Timer(tm, deviceId, timeout, timeout);
            while (JsonStrings[deviceId] == null)
            {
                // loop
            }
            timer.Dispose();
            Connections.Remove(deviceId);
            return "";
        }

        private void Count(object obj)
        {
            string deviceId = (string) obj;
            JsonStrings[deviceId] = "timeout";
        }
    }             
}