using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IAlbumRepository
    {
        LibraryAlbum GetAlbum(long id);

        void SaveAlbum(LibraryAlbum Album);

        void SaveAlbums(IEnumerable<LibraryAlbum> Albums);

        IEnumerable<LibraryAlbum> GetAllAlbums();

        IEnumerable<LibraryAlbum> GetAlbumPage(int offset, int limit);

        IEnumerable<LibraryAlbum> GetUpdatedAlbums(int offset, int limit, long epoch);

        IEnumerable<LibraryAlbum> GetCachedAlbums();

        IEnumerable<LibraryAlbum> GetDeletedAlbums();

        void DeleteAlbums(IEnumerable<LibraryAlbum> Albums);

        int GetAlbumCount();
    }
}
