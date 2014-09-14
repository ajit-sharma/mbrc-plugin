using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;

namespace MusicBeePlugin.Rest.ServiceInterface
{
    class LibraryService : Service
    {
        public PaginatedResult Get(GetLibraryTracks request)
        {
            return Plugin.Instance.LibraryModule.GetAllTracks(request.limit, request.offset);
        }

        public LibraryTrack Get(GetLibraryTrack request)
        {
            return Plugin.Instance.LibraryModule.GetTrackById(request.id);
        }

        public PaginatedResult Get(GetLibraryArtists request)
        {
            return Plugin.Instance.LibraryModule.GetAllArtists(request.limit, request.offset);
        }

        public LibraryArtist Get(GetLibraryArtist request)
        {
            return Plugin.Instance.LibraryModule.GetArtistById(request.id);
        }

        public PaginatedResult Get(GetLibraryGenres request)
        {
            return Plugin.Instance.LibraryModule.GetAllGenres(request.limit, request.offset);
        }

        public PaginatedResult Get(GetLibraryAlbums request)
        {
            return Plugin.Instance.LibraryModule.GetAllAlbums(request.limit, request.offset);
        }
    }
}
