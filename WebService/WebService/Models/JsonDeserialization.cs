using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WebService.Models
{
    public class JsonDeserialization
    {
        public static GetCommandDto Deserialization(Stream stream)
        {
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof (GetCommandDto));
                GetCommandDto getCommand = (GetCommandDto) ser.ReadObject(stream);
                return getCommand;
            }
            catch (SerializationException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        [DataContract]
        public class GetCommandDto
        {
            [DataMember]
            public int CommandId { get; set; }
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