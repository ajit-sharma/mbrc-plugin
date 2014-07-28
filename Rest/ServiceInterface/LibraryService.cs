using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;
using System.Collections.Generic;

namespace MusicBeePlugin.Rest.ServiceInterface
{
    class LibraryService : Service
    {
        public List<LibraryTrack> Get(GetLibraryTracks request)
        {
            return Plugin.Instance.LibraryModule.GetAllTracks();
        }

        public LibraryTrack Get(GetLibraryTrack request)
        {
            return Plugin.Instance.LibraryModule.GetTrackById(request.id);
        }

        public List<LibraryArtist> Get(GetLibraryArtists request)
        {
            return Plugin.Instance.LibraryModule.GettAllArtists();
        }

        public LibraryArtist Get(GetLibraryArtist request)
        {
            return Plugin.Instance.LibraryModule.GetArtistById(request.id);
        }
    }
}
