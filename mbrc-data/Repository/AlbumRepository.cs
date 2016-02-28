namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class AlbumRepository : IAlbumRepository
    {
        public int Delete(IList<LibraryAlbum> Albums)
        {
            throw new System.NotImplementedException();
        }

        public int SoftDelete(IList<LibraryAlbum> t)
        {
            throw new System.NotImplementedException();
        }

        public LibraryAlbum GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetCount()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryAlbum> GetPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryAlbum> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryAlbum> GetCached()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryAlbum> GetDeleted()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryAlbum> GetUpdatedPage(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public int Save(LibraryAlbum Album)
        {
            throw new System.NotImplementedException();
        }

        public int Save(IList<LibraryAlbum> Albums)
        {
            throw new System.NotImplementedException();
        }
    }
}