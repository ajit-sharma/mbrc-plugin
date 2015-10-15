#region

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints.Support;

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

        public PaginatedResponse<Playlist> Get(AllPlaylists request)
        {
            return _module.GetAvailablePlaylists(request.Limit, request.Offset, request.After);
        }

        public PaginatedResponse<PlaylistTrack> Get(GetPlaylistTracks request)
        {
            return _module.GetPlaylistTracks(request.Id, request.Limit, request.Offset, request.After);
        }

	    public PaginatedResponse<PlaylistTrackInfo> Get(GetPlaylistTrackInfo request)
	    {
		    return _module.GetPlaylistTracksInfo(request.Limit, request.Offset, request.After);
	    } 

        public ResponseBase Put(CreatePlaylist request)
        {
            return _module.CreateNewPlaylist(request.Name, request.List);
        }

        public ResponseBase Put(PlaylistPlay request)
        {
            return _module.PlaylistPlayNow(request.Path);
        }

        public ResponseBase Put(AddPlaylistTracks request)
        {
            return new ResponseBase
            {
                Code = _module.PlaylistAddTracks(request.Id, request.List) ? ApiCodes.Success : ApiCodes.Failure
            };
        }

        public ResponseBase Delete(DeletePlaylist request)
        {
            return new ResponseBase
            {
                Code = _module.PlaylistDelete(request.Id) ? ApiCodes.Success : ApiCodes.Failure
            };
        }

        public ResponseBase Delete(DeletePlaylistTracks request)
        {
            return new ResponseBase
            {
                Code = _module.DeleteTrackFromPlaylist(request.Id, request.Position) ? ApiCodes.Success : ApiCodes.Failure
            };
        }

        public ResponseBase Put(MovePlaylistTrack request)
        {
            return new ResponseBase
            {
                Code = _module.MovePlaylistTrack(request.Id, request.From, request.To) ? ApiCodes.Success : ApiCodes.Failure
            };
        }
	}
}