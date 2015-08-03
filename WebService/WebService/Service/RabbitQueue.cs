using System;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using RabbitMQ.Client;
using IConnection = RabbitMQ.Client.IConnection;

namespace WebService.Service
{
    public class RabbitQueue
    {
        private static int _commandId;
        public int TimeoutMs = Convert.ToInt32(WebConfigurationManager.AppSettings["RabbitMQTimeoutCommand"]);

        public void Producer(string queueName, string command)
        {
            var connectionFactory = new ConnectionFactory
                {
                    UserName = WebConfigurationManager.AppSettings["RabbitMQUserName"],
                    Password = WebConfigurationManager.AppSettings["RabbitMQPassword"],
                    VirtualHost = WebConfigurationManager.AppSettings["RabbitMQVirtualHost"],
                    HostName = WebConfigurationManager.AppSettings["RabbitMQHostName"],
                    Port = Convert.ToInt32(WebConfigurationManager.AppSettings["RabbitMQPort"])
                };
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            string exchangeName = WebConfigurationManager.AppSettings["RabbitMQExchangeName"];

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, true, null);
            channel.QueueBind(queueName, exchangeName, queueName);

            byte[] message = Encoding.UTF8.GetBytes(command);
            channel.BasicPublish(exchangeName, queueName, null, message);

            channel.Close();
            connection.Close();
        }

        public string Consumer(string queueName)
        {
            try
            {
                var connectionFactory = new ConnectionFactory
                {
                    UserName = WebConfigurationManager.AppSettings["RabbitMQUserName"],
                    Password = WebConfigurationManager.AppSettings["RabbitMQPassword"],
                    VirtualHost = WebConfigurationManager.AppSettings["RabbitMQVirtualHost"],
                    HostName = WebConfigurationManager.AppSettings["RabbitMQHostName"],
                    Port = Convert.ToInt32(WebConfigurationManager.AppSettings["RabbitMQPort"])
                };
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
                    string result = Logs.Save(queueName, Encoding.UTF8.GetString(command.Body), "sent", _commandId);

                    if (result != "OK")
                        return result;
                
                    if (_commandId != 255)
                        _commandId++;
                    else
                        _commandId = 0;
                    return Encoding.UTF8.GetString(command.Body);
                }
            }
            catch
            {
                return "RabbitMQ error";
            }
            return null;
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
                var connectionFactory = new ConnectionFactory
                {
                    UserName = WebConfigurationManager.AppSettings["RabbitMQUserName"],
                    Password = WebConfigurationManager.AppSettings["RabbitMQPassword"],
                    VirtualHost = WebConfigurationManager.AppSettings["RabbitMQVirtualHost"],
                    HostName = WebConfigurationManager.AppSettings["RabbitMQHostName"],
                    Port = Convert.ToInt32(WebConfigurationManager.AppSettings["RabbitMQPort"])
                };
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
                    Logs.Save(queueName, Encoding.UTF8.GetString(command.Body), "timeout", _commandId);
                    if (_commandId != 255)
                        _commandId++;
                    else
                        _commandId = 0;
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
