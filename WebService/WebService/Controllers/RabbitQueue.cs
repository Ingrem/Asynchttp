using System;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;

namespace WebService.Controllers
{
    public class RabbitQueue
    {
        private int _commandId;
        public const int TimeoutMs = 10000;
        public void Producer(string queueName, string command)
        {
            var connectionFactory = new ConnectionFactory();
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            string exchangeName = WebConfigurationManager.AppSettings["RabbitMQExchangeName"];

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, true, null);
            channel.QueueBind(queueName, exchangeName, queueName);

            ChangeCommandIdWithDeviceId(ref command);

            byte[] message = Encoding.UTF8.GetBytes(command);
            channel.BasicPublish(exchangeName, queueName, null, message);

            channel.Close();
            connection.Close();
        }

        //Change (DeviceId:"{DeviceId}") with (CommandId:"{CommandId}")
        public void ChangeCommandIdWithDeviceId(ref string json)
        {
            const int commandLen = 11; // DeviceId:"id"
            int startId = json.IndexOf("DeviceId", StringComparison.Ordinal) + commandLen;
            int endId = startId;
            while (json[endId] != '"')
                endId++;

            // change DeviceId with CommandId
            json = json.Remove(startId - commandLen, endId - startId + commandLen);
            json = json.Insert(startId - commandLen, String.Format("CommandId\":\"{0}", _commandId));
            if (_commandId != 255)
                _commandId++;
            else
                _commandId = 0;
        }

        public void Consumer(string queueName)
        {
            try
            {
                var connectionFactory = new ConnectionFactory();
                IConnection connection = connectionFactory.CreateConnection();
                IModel channel = connection.CreateModel();
                channel.QueueDeclare(queueName, false, false, true, null);

                var command = channel.BasicGet(queueName, true);
                if (channel.BasicGet(queueName, false) == null)
                    channel.QueueDelete(queueName);

                channel.Close();
                connection.Close();

                if (command != null)
                {
                    Logs.Save(queueName, Encoding.UTF8.GetString(command.Body), "sent");
                    //request!!!!
                    //return status code and json then close polling
                }
            }
            catch
            {
                // ignored
            }
        }
        public void CreateTimeout(string queueName)
        {
            TimerCallback tm = Count;
            new Timer(tm, queueName, TimeoutMs, TimeoutMs);
        }
        private void Count(object obj)
        {
            try
            {
                var queueName = (string)obj;
                var connectionFactory = new ConnectionFactory();
                IConnection connection = connectionFactory.CreateConnection();
                IModel channel = connection.CreateModel();
                channel.QueueDeclare(queueName, false, false, true, null);

                var command = channel.BasicGet(queueName, true);
                if (channel.BasicGet(queueName, false) == null)
                    channel.QueueDelete(queueName);

                channel.Close();
                connection.Close();

                if (command != null)
                    Logs.Save(queueName, Encoding.UTF8.GetString(command.Body), "timeout");
            }
            catch
            {
                // ignored
            }
        }
    }
}
