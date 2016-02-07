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

        void SaveArtists(ICollection<LibraryArtist> artists);

        ICollection<LibraryArtist> GetAllArtists();

        ICollection<LibraryArtist> GetArtistPage(int offset, int limit);

        ICollection<LibraryArtist> GetUpdatedArtists(int offset, int limit, long epoch);

        ICollection<LibraryArtist> GetCachedArtists();

        ICollection<LibraryArtist> GetDeletedArtists();

        void DeleteArtists(ICollection<LibraryArtist> artists);

        int GetArtistCount();
    }
}
