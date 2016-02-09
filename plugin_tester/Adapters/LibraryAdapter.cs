namespace plugin_tester
{
    using System;
    using System.Collections.Generic;

    using MusicBeePlugin;
    using MusicBeePlugin.Model;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    internal class LibraryAdapter: ILibraryApiAdapter
    {
        public ICollection<LibraryAlbum> GetAlbumList()
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryArtist> GetArtistList()
        {
            throw new NotImplementedException();
        }

        public string GetArtistUrl(string name)
        {
            throw new NotImplementedException();
        }

        public byte[] GetCoverData(string path)
        {
            throw new NotImplementedException();
        }

        public string GetCoverUrl(string path)
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryGenre> GetGenreList()
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