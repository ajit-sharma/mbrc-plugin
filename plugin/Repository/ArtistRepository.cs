namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    class ArtistRepository : IArtistRepository
    {
        public LibraryArtist GetArtist(long id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveArtist(LibraryArtist artist)
        {
            throw new System.NotImplementedException();
        }

        public void SaveArtists(IEnumerable<LibraryArtist> artists)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryArtist> GetAllArtists()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryArtist> GetArtistPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryArtist> GetUpdatedArtists(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryArtist> GetCachedArtists()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryArtist> GetDeletedArtists()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteArtists(IEnumerable<LibraryArtist> artists)
        {
            throw new System.NotImplementedException();
        }

        public int GetArtistCount()
        {
            throw new System.NotImplementedException();
        }
    }
}