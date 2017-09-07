using MusicBeeRemote.Core.ApiAdapters;
using Nancy;

namespace MusicBeeRemote.Core.Network.Http.Api
{
    public class OutputModule : NancyModule
    {
        public OutputModule(IOutputApiAdapter apiAdapter) : base("/output")
        {
            Get["/"] = parameters =>
            {
                return apiAdapter.GetOutputDevices();
            };
        }
    }
}