using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using WebService.Models;

namespace WebService.Controllers
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

                if (getCommand.Command.CommandName == "delete" || getCommand.Command.CommandName == "getInfo")
                    return true;
                if (getCommand.Command.CommandName != "upgrade" && getCommand.Command.CommandName != "setOnOff")
                    return false;
                switch (getCommand.Command.CommandName)
                {
                    case "upgrade":
                        return
                            getCommand.Command.Parameters.All(a => (a.Key == "name" && (string) a.Value == "url") ||
                                                                   (a.Key == "value" && ((string) a.Value != "")));
                    case "setOnOff":
                        return 
                            getCommand.Command.Parameters.All(a => (a.Key == "name" && (string) a.Value == "switchOn") ||
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
