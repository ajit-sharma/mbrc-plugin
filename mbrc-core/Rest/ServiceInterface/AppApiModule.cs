using MusicBeeRemoteCore.Rest.ServiceModel.Type;
using Nancy;

namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    public class AppApiModule : NancyModule
    {
        public AppApiModule()
        {
            Get["/"] = _ => Response.AsJson(new ResponseBase {Code = ApiCodes.Success});
            Get["/status"] = _ => Response.AsJson(new ResponseBase { Code = ApiCodes.Success });
        }
    }
}