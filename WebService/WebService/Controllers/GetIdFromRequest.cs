using System;

namespace WebService.Controllers
{
    public class GetIdFromRequest
    {
        public string FromGet(string url)
        {
            int i = 10; // DeviceId/{id}/
            while (url[i] != '/')
                i++;
            return url.Substring(10, i - 10);
        }

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
