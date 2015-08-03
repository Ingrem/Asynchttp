using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace WebService.Service
{
    public class Logs
    {
        public static string Save(string deviceId, string jsonString, string commandStatus, int commandId)
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["Mongodb"].ConnectionString;
                MongoClient client = new MongoClient(connectionString);
                MongoServer server = client.GetServer();
                MongoDatabase database = server.GetDatabase(WebConfigurationManager.AppSettings["DatabaseName"]);
                MongoCollection<GetCommandDto> col = database.GetCollection<GetCommandDto>(deviceId);

                byte[] byteArray = Encoding.Unicode.GetBytes(jsonString);
                MemoryStream stream = new MemoryStream(byteArray);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (GetCommandDto));
                GetCommandDto getCommand = (GetCommandDto) serializer.ReadObject(stream);
                getCommand.CommandStatus = commandStatus;
                getCommand.Id = ObjectId.GenerateNewId().ToString();
                getCommand.CommandId = commandId;

                col.Insert(getCommand);
            }
            catch
            {
                return "Database error";
            }
            return "OK";
        }

        public static string ReturnLogs()
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["Mongodb"].ConnectionString;
                MongoClient client = new MongoClient(connectionString);
                MongoServer server = client.GetServer();
                MongoDatabase database = server.GetDatabase(WebConfigurationManager.AppSettings["DatabaseName"]);
                StringBuilder all = new StringBuilder();
                try
                {
                    foreach (var c in database.GetCollectionNames().Where(c => c != "system.indexes"))
                    {
                        all.Append(String.Format("Device ID - {0} \r\n", c));
                        foreach (GetCommandDto com in LogsForDevice(c, database))
                        {
                            all.Append(String.Format("  Command ID - {0} ", com.CommandId));
                            all.Append(String.Format("Name - {0} ", com.CommandName));
                            all = com.Parameters.Values.Aggregate(all, (current, a) => current.Append(a + " "));
                            all.Append(String.Format("Status - {0}", com.CommandStatus));
                            all.Append("\r\n");
                        }
                    }
                    return all.ToString();
                }
                catch
                {
                    return all.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        private static IEnumerable<GetCommandDto> LogsForDevice(string deviceId, MongoDatabase database)
        {
            try
            {
                MongoCollection<GetCommandDto> col = database.GetCollection<GetCommandDto>(deviceId);
                IEnumerable<GetCommandDto> com = col.FindAll();
                return com;
            }
            catch
            {
                return null;
            }
        }

        [DataContract]
        public class GetCommandDto
        {
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            [DataMember]
            public string CommandStatus { get; set; }
            [DataMember]
            public int CommandId { get; set; }
            [DataMember]
            public string CommandName { get; set; }
            [DataMember]
            public Dictionary<string, object> Parameters { get; set; }
        } 
    }
}
