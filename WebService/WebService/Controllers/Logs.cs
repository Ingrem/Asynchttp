﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Web.Configuration;

namespace WebService.Controllers
{
    public class Logs
    {
        static readonly string ConnectionString = WebConfigurationManager.ConnectionStrings["Mongodb"].ConnectionString;
        static readonly MongoClient Client = new MongoClient(ConnectionString);
        static readonly MongoServer Server = Client.GetServer();
        static readonly MongoDatabase Database = Server.GetDatabase(WebConfigurationManager.AppSettings["DatabaseName"]);

        public static void Save(string deviceId, string jsonString, string commandStatus, int commandId)
        {
            MongoCollection<GetCommandDto> col = Database.GetCollection<GetCommandDto>(deviceId);

            byte[] byteArray = Encoding.Unicode.GetBytes(jsonString);
            MemoryStream stream = new MemoryStream(byteArray);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (GetCommandDto));
            GetCommandDto getCommand = (GetCommandDto) serializer.ReadObject(stream);
            getCommand.CommandStatus = commandStatus;
            getCommand.Id = ObjectId.GenerateNewId().ToString();
            getCommand.CommandId = commandId;

            col.Insert(getCommand);
        }

        public static string ReturnLogs()
        {
            StringBuilder all = new StringBuilder();
            try
            {
                foreach (var c in Database.GetCollectionNames().Where(c => c != "system.indexes"))
                {
                    all.Append(String.Format("Device ID - {0} \r\n", c));
                    foreach (GetCommandDto com in LogsForDevice(c))
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

        private static IEnumerable<GetCommandDto> LogsForDevice(string deviceId)
        {
            try
            {
                MongoCollection<GetCommandDto> col = Database.GetCollection<GetCommandDto>(deviceId);
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
