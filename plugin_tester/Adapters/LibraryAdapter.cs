using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Model;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace plugin_tester
{
    using System;
    using System.Collections.Generic;
    using MusicBeeRemoteData.Entities;

    internal class LibraryAdapter : ILibraryApiAdapter
    {
        public ICollection<AlbumDao> GetAlbums()
        {
            throw new NotImplementedException();
        }

        public ICollection<ArtistDao> GetArtists()
        {
            throw new NotImplementedException();
        }

        public string GetArtistUrl(string name)
        {
            throw new NotImplementedException();
        }

        public byte[] GetCover(string path)
        {
            throw new NotImplementedException();
        }

        public string GetCoverUrl(string path)
        {
            throw new NotImplementedException();
        }

        public ICollection<GenreDao> GetGenres()
        {
            throw new NotImplementedException();
        }

        public string[] GetLibraryFiles()
        {
            throw new NotImplementedException();
        }

        public Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync)
        {
            throw new NotImplementedException();
        }

        public LibraryTrackEx GetTags(string file)
        {
            throw new NotImplementedException();
        }
    }
}