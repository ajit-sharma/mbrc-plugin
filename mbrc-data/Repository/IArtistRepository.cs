namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public interface IArtistRepository
    {
        void DeleteArtists(ICollection<LibraryArtist> artists);

        ICollection<LibraryArtist> GetAllArtists();

        LibraryArtist GetArtist(long id);

        int GetArtistCount();

        ICollection<LibraryArtist> GetArtistPage(int offset, int limit);

        ICollection<LibraryArtist> GetCachedArtists();

        ICollection<LibraryArtist> GetDeletedArtists();

        ICollection<LibraryArtist> GetUpdatedArtists(int offset, int limit, long epoch);

        void SaveArtist(LibraryArtist artist);

        void SaveArtists(ICollection<LibraryArtist> artists);
    }
}