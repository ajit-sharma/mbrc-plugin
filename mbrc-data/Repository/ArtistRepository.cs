namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    public class ArtistRepository : IArtistRepository
    {
        public void DeleteArtists(ICollection<LibraryArtist> artists)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryArtist> GetAllArtists()
        {
            throw new System.NotImplementedException();
        }

        public LibraryArtist GetArtist(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetArtistCount()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryArtist> GetArtistPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryArtist> GetCachedArtists()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryArtist> GetDeletedArtists()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryArtist> GetUpdatedArtists(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public void SaveArtist(LibraryArtist artist)
        {
            throw new System.NotImplementedException();
        }

        public void SaveArtists(ICollection<LibraryArtist> artists)
        {
            throw new System.NotImplementedException();
        }
    }
}