#region

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class PlaylistService : Service
    {
        private readonly PlaylistModule _module;

        public PlaylistService(PlaylistModule module)
        {
            _module = module;
        }

        public PaginatedResponse Get(AllPlaylists request)
        {
            return _module.GetAvailablePlaylists(request.limit, request.offset);
        }

        public PaginatedResponse Get(GetPlaylistTracks request)
        {
            return _module.GetPlaylistTracks(request.id);
        }

        public SuccessResponse Put(CreatePlaylist request)
        {
            return _module.CreateNewPlaylist(request.name, request.list);
        }

        public SuccessResponse Put(PlaylistPlay request)
        {
            return _module.PlaylistPlayNow(request.path);
        }

        public SuccessResponse Put(AddPlaylistTracks request)
        {
            return new SuccessResponse
            {
                success = _module.PlaylistAddTracks(request.id, request.list)
            };
        }

        public SuccessResponse Delete(DeletePlaylist request)
        {
            return new SuccessResponse
            {
                success = _module.PlaylistDelete(request.id)
            };
        }

        public SuccessResponse Delete(DeletePlaylistTracks request)
        {
            return new SuccessResponse
            {
                success = _module.DeleteTrackFromPlaylist(request.id, request.index)
            };
        }

        public SuccessResponse Patch(MovePlaylistTrack request)
        {
            return new SuccessResponse
            {
                success = _module.MovePlaylistTrack(request.id, request.from, request.to)
            };
        }
    }
}