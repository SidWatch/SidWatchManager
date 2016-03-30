using Nancy;

namespace SidWatchCollectionLibrary.Modules
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
        }
    }
}
