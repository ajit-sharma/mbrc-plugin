#region

using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class PlaylistService : Service
    {
        public PaginatedResponse Get(AllPlaylists request)
        {
            return Plugin.Instance.PlaylistModule.GetAvailablePlaylists(request.limit, request.offset);
        }

        public PaginatedResponse Get(GetPlaylistTracks request)
        {
            return Plugin.Instance.PlaylistModule.GetPlaylistTracks(request.id);
        }

        public SuccessResponse Put(CreatePlaylist request)
        {
            return Plugin.Instance.PlaylistModule.RequestPlaylist(request.name, request.list);
        }

        public SuccessResponse Put(PlaylistPlay request)
        {
            return Plugin.Instance.PlaylistModule.PlaylistPlayNow(request.path);
        }

        public SuccessResponse Put(AddPlaylistTracks request)
        {
            return new SuccessResponse
            {
                success = Plugin.Instance.PlaylistModule.PlaylistAddTracks(request.id, request.list)
            };
        }
    }
}