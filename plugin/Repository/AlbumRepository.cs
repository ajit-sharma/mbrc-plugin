namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    class AlbumRepository : IAlbumRepository
    {
        public LibraryAlbum GetAlbum(long id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveAlbum(LibraryAlbum Album)
        {
            throw new System.NotImplementedException();
        }

        public void SaveAlbums(IEnumerable<LibraryAlbum> Albums)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryAlbum> GetAllAlbums()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryAlbum> GetAlbumPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryAlbum> GetUpdatedAlbums(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryAlbum> GetCachedAlbums()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryAlbum> GetDeletedAlbums()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAlbums(IEnumerable<LibraryAlbum> Albums)
        {
            throw new System.NotImplementedException();
        }

        public int GetAlbumCount()
        {
            throw new System.NotImplementedException();
        }
    }
}