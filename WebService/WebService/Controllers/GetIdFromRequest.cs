using System;

namespace WebService.Controllers
{
    public class GetIdFromRequest
    {
        public string FromPost(string json)
        {
            int startId = json.IndexOf("DeviceId", StringComparison.Ordinal) + 11; // DeviceId:"id"
            int endId = startId;
            while (json[endId] != '"')
                endId++;

            return json.Substring(startId, endId - startId);
        }
    }
}
