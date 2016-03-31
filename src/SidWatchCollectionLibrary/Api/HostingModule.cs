using System.IO;
using System.Text;
using Nancy;
using Newtonsoft.Json;
using SidWatchLibrary.Objects;

namespace SidWatchCollectionLibrary.Api
{
    public class HostingModule : NancyModule
    {
        public HostingModule()
        {
            // would capture routes to /products/list sent as a GET request
            Get["/"] = _parameters =>
            {
                return "SidWatch API";
            };

            Get["/lastsegment"] = _parameters =>
            {
                AudioSegment segment = DataCache.GetInstance().LastAudioSegment;

                string json = JsonConvert.SerializeObject(segment);

                var jsonBytes = Encoding.UTF8.GetBytes(json);

                return new Response
                {
                    ContentType = "application/json",
                    Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                };
            };
        }
    }
}
