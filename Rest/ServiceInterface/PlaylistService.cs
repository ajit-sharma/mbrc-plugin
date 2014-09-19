using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;
using MusicBeePlugin.Rest.ServiceModel;

namespace MusicBeePlugin.Rest.ServiceInterface
{
    class PlaylistService : Service
    {
        public PaginatedResult Get(AllPlaylists request)
        {
            return Plugin.Instance.PlaylistModule.GetAvailablePlaylists(request.limit, request.offset);
        }
    }
}
