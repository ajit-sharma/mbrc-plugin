using MusicBeeRemote.Core.Network.Http.Responses.Type;
using MusicBeeRemote.Core.Settings;
using Nancy;

namespace MusicBeeRemote.Core.Network.Http.Api
{
    public class AppApiModule : NancyModule
    {
        public AppApiModule(PersistenceManager manager)
        {
            Get["/"] = _ => Response.AsJson(new ApiResponse());
            Get["/status"] = _ => Response.AsJson(new ApiResponse());
            Get["/settings"] = _ => Response.AsJson(manager.UserSettingsModel);
        }
    }
}