namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public interface IAlbumRepository
    {
        void DeleteAlbums(ICollection<LibraryAlbum> Albums);

        LibraryAlbum GetAlbum(long id);

        int GetAlbumCount();

        ICollection<LibraryAlbum> GetAlbumPage(int offset, int limit);

        ICollection<LibraryAlbum> GetAllAlbums();

        ICollection<LibraryAlbum> GetCachedAlbums();

        ICollection<LibraryAlbum> GetDeletedAlbums();

        ICollection<LibraryAlbum> GetUpdatedAlbums(int offset, int limit, long epoch);

        void SaveAlbum(LibraryAlbum Album);

        void SaveAlbums(ICollection<LibraryAlbum> Albums);
    }
}