namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public class AlbumRepository : IAlbumRepository
    {
        public void DeleteAlbums(ICollection<LibraryAlbum> Albums)
        {
            throw new System.NotImplementedException();
        }

        public LibraryAlbum GetAlbum(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetAlbumCount()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryAlbum> GetAlbumPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryAlbum> GetAllAlbums()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryAlbum> GetCachedAlbums()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryAlbum> GetDeletedAlbums()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryAlbum> GetUpdatedAlbums(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public void SaveAlbum(LibraryAlbum Album)
        {
            throw new System.NotImplementedException();
        }

        public void SaveAlbums(ICollection<LibraryAlbum> Albums)
        {
            throw new System.NotImplementedException();
        }
    }
}