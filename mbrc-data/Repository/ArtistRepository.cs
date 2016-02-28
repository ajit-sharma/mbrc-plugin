namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class ArtistRepository : IArtistRepository
    {
        public int Delete(IList<LibraryArtist> artists)
        {
            throw new System.NotImplementedException();
        }

        public int SoftDelete(IList<LibraryArtist> t)
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryArtist> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public LibraryArtist GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetCount()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryArtist> GetPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryArtist> GetCached()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryArtist> GetDeleted()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryArtist> GetUpdatedPage(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public int Save(LibraryArtist artist)
        {
            throw new System.NotImplementedException();
        }

        public int Save(IList<LibraryArtist> artists)
        {
            throw new System.NotImplementedException();
        }
    }
}