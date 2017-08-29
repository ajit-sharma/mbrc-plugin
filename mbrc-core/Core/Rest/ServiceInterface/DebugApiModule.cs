using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using Nancy;

namespace MusicBeeRemote.Core.Rest.ServiceInterface
{
    /// <summary>
    ///     Service Responsible for Debug
    /// </summary>
    public class DebugApiModule : NancyModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugApiModule" /> class.
        /// </summary>
        public DebugApiModule()
            : base("/debug")
        {
            Get["/"] = _ => Response.AsJson(new ResponseBase {Code = ApiCodes.Success});
        }
    }
}