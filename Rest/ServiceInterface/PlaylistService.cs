#region

using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Ninject;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class PlaylistService : Service
    {
        private PlaylistModule module;

        public PlaylistService()
        {
            var kernel = new StandardKernel(new InjectionModule());
            module = kernel.Get<PlaylistModule>();
        }

        public PaginatedResponse Get(AllPlaylists request)
        {
            return module.GetAvailablePlaylists(request.limit, request.offset);
        }

        public PaginatedResponse Get(GetPlaylistTracks request)
        {
            return module.GetPlaylistTracks(request.id);
        }

        public SuccessResponse Put(CreatePlaylist request)
        {
            return module.CreateNewPlaylist(request.name, request.list);
        }

        public SuccessResponse Put(PlaylistPlay request)
        {
            return module.PlaylistPlayNow(request.path);
        }

        public SuccessResponse Put(AddPlaylistTracks request)
        {
            return new SuccessResponse
            {
                success = module.PlaylistAddTracks(request.id, request.list)
            };
        }

        public SuccessResponse Delete(DeletePlaylist request)
        {
            return new SuccessResponse
            {
                success = module.PlaylistDelete(request.id)
            };
        }

        public SuccessResponse Delete(DeletePlaylistTracks request)
        {
            return new SuccessResponse
            {
                success = module.DeleteTrackFromPlaylist(request.id, request.index)
            };
        }
    }
}