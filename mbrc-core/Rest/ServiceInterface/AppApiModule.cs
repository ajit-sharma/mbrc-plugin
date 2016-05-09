using MusicBeeRemoteCore.Rest.ServiceModel.Type;
using Nancy;

namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    internal class AppApiModule : NancyModule
    {
        public AppApiModule()
        {
            Get["/"] = _ => Response.AsJson(new ResponseBase {Code = ApiCodes.Success});
        }
    }
}