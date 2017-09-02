using MusicBeeRemote.Core.Network.Http.Responses.Type;
using Nancy;

namespace MusicBeeRemote.Core.Network.Http.Api
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