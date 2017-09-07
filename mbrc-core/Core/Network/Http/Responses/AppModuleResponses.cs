using System.Runtime.Serialization;
using MusicBeeRemote.Core.Network.Http.Responses.Type;
using Nancy.Routing;

namespace MusicBeeRemote.Core.Network.Http.Responses
{
    [DataContract]
    public class RouteResponse : ApiResponse
    {
        [DataMember(Name = "routes")]
        public IRouteCache Routes { get; set; }
    }
}