#region

using System.IO;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class LibraryService : Service
    {
        private readonly LibraryModule _module;

        public LibraryService(LibraryModule module)
        {
            _module = module;
        }

        public PaginatedResponse Get(GetLibraryTracks request)
        {
            return _module.GetAllTracks(request.limit, request.offset);
        }

        public LibraryTrack Get(GetLibraryTrack request)
        {
            return _module.GetTrackById(request.id);
        }

        public PaginatedResponse Get(GetLibraryArtists request)
        {
            return _module.GetAllArtists(request.limit, request.offset);
        }

        public LibraryArtist Get(GetLibraryArtist request)
        {
            return _module.GetArtistById(request.id);
        }

        public PaginatedResponse Get(GetLibraryGenres request)
        {
            return _module.GetAllGenres(request.limit, request.offset);
        }

        public PaginatedResponse Get(GetLibraryAlbums request)
        {
            return _module.GetAllAlbums(request.limit, request.offset);
        }

        public PaginatedResponse Get(GetLibraryCovers request)
        {
            return _module.GetAllCovers(request.offset, request.limit);
        }

        public LibraryCover Get(GetLibraryCover request)
        {
            return _module.GetLibraryCover(request.id, true);
        }

        [AddHeader(ContentType = "image/jpeg")]
        public Stream Get(GetLibraryCoverData request)
        {
            return _module.GetCoverData(request.id);
        }
    }
}