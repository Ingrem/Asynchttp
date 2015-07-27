using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WebService.Models
{
    public class JsonSerialization
    {
        public static string Serialization(SendCommandDto json)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(SendCommandDto));
                ser.WriteObject(stream, json);
                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        [DataContract]
        public class SendCommandDto
        {
            [DataMember]
            public string DeviceId { get; set; }
            [DataMember]
            public CommandField Command { get; set; }
        }

        [DataContract]
        public class CommandField
        {
            [DataMember]
            public string CommandName { get; set; }
            [DataMember]
            public Dictionary<string,object> Parameters { get; set; }
        } 
    }
}