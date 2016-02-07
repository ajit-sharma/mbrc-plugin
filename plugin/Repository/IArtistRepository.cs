using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IArtistRepository
    {
        LibraryArtist GetArtist(long id);

        void SaveArtist(LibraryArtist artist);

        void SaveArtists(IEnumerable<LibraryArtist> artists);

        IEnumerable<LibraryArtist> GetAllArtists();

        IEnumerable<LibraryArtist> GetArtistPage(int offset, int limit);

        IEnumerable<LibraryArtist> GetUpdatedArtists(int offset, int limit, long epoch);

        IEnumerable<LibraryArtist> GetCachedArtists();

        IEnumerable<LibraryArtist> GetDeletedArtists();

        void DeleteArtists(IEnumerable<LibraryArtist> artists);

        int GetArtistCount();
    }
}
