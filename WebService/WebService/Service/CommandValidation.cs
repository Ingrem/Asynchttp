using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using WebService.Models;

namespace WebService.Service
{
    public class CommandValidation
    {
        public bool Validation(string jsonString)
        {
            try
            {
                byte[] byteArray = Encoding.Unicode.GetBytes(jsonString);
                MemoryStream stream = new MemoryStream(byteArray);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonDeserialization.GetCommandDto));
                JsonDeserialization.GetCommandDto getCommand = (JsonDeserialization.GetCommandDto)serializer.ReadObject(stream);

                if (getCommand.CommandName == "delete" || getCommand.CommandName == "getInfo")
                    return true;
                if (getCommand.CommandName != "upgrade" && getCommand.CommandName != "setOnOff")
                    return false;
                switch (getCommand.CommandName)
                {
                    case "upgrade":
                        return
                            getCommand.Parameters.All(a => (a.Key == "name" && (string) a.Value == "url") ||
                                                                   (a.Key == "value" && ((string) a.Value != "")));
                    case "setOnOff":
                        return 
                            getCommand.Parameters.All(a => (a.Key == "name" && (string) a.Value == "switchOn") ||
                                                                      (a.Key == "value" && Convert.ToBoolean(a.Value) is bool));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
