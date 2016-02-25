namespace MusicBeePlugin.ApiAdapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MusicBeeRemoteCore;
    using MusicBeeRemoteCore.Model;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// The library api adapter.
    /// </summary>
    public class LibraryApiAdapter : ILibraryApiAdapter
    {
        private readonly Plugin.MusicBeeApiInterface api;

        /// <summary>
        ///     The fields (MetaData) that MusicBee Remote caches.
        /// </summary>
        private readonly Plugin.MetaDataType[] fields =
            {
                Plugin.MetaDataType.Artist, Plugin.MetaDataType.AlbumArtist, 
                Plugin.MetaDataType.Album, Plugin.MetaDataType.Genre, 
                Plugin.MetaDataType.TrackTitle, Plugin.MetaDataType.Year, 
                Plugin.MetaDataType.TrackNo, Plugin.MetaDataType.DiscNo
            };

        public LibraryApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            this.api = api;
        }

        public ICollection<LibraryAlbum> GetAlbumList()
        {
            var list = new List<LibraryAlbum>();
            if (this.api.Library_QueryLookupTable("album", "count", null))
            {
                list.AddRange(
                    this.api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] { "\0\0" }, StringSplitOptions.None)
                        .Select(artist => new LibraryAlbum { Name = artist.Split('\0')[0] }));
            }

            this.api.Library_QueryLookupTable(null, null, null);
            return list;
        }

        public ICollection<LibraryArtist> GetArtistList()
        {
            var artists = new List<LibraryArtist>();
            if (this.api.Library_QueryLookupTable("artist", "count", string.Empty))
            {
                artists.AddRange(
                    this.api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] { "\0\0" }, StringSplitOptions.None)
                        .Select(entry => entry.Split('\0'))
                        .Select(artistInfo => new LibraryArtist(artistInfo[0])));
            }

            this.api.Library_QueryLookupTable(null, null, null);

            return artists;
        }

        public string GetArtistUrl(string name)
        {
            string[] urls = { };
            this.api.Library_GetArtistPictureUrls(name, true, ref urls);
            return urls.Length > 0 ? urls[0] : string.Empty;
        }

        public byte[] GetCoverData(string path)
        {
            string coverUrl = null;
            var locations = Plugin.PictureLocations.None;
            byte[] imageData = { };
            this.api.Library_GetArtworkEx(path, 0, true, ref locations, ref coverUrl, ref imageData);
            return imageData;
        }

        public string GetCoverUrl(string path)
        {
            string coverUrl = null;
            var locations = Plugin.PictureLocations.None;
            byte[] imageData = { };
            this.api.Library_GetArtworkEx(path, 0, false, ref locations, ref coverUrl, ref imageData);
            return coverUrl;
        }

        public ICollection<LibraryGenre> GetGenreList()
        {
            var list = new List<LibraryGenre>();
            if (this.api.Library_QueryLookupTable("genre", "count", null))
            {
                list.AddRange(
                    this.api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] { "\0\0" }, StringSplitOptions.None)
                        .Select(artist => new LibraryGenre(artist.Split('\0')[0])));
            }

            this.api.Library_QueryLookupTable(null, null, null);
            return list;
        }

        public string[] GetLibraryFiles()
        {
            string[] files = { };
            this.api.Library_QueryFilesEx(string.Empty, ref files);
            return files;
        }

        public Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync)
        {
            string[] newFiles = { };
            string[] deletedFiles = { };
            string[] updatedFiles = { };

            this.api.Library_GetSyncDelta(
                cachedFiles, 
                lastSync, 
                Plugin.LibraryCategory.Music, 
                ref newFiles, 
                ref updatedFiles, 
                ref deletedFiles);
            return new Modifications(deletedFiles, newFiles, updatedFiles);
        }

        public LibraryTrackEx GetTags(string file)
        {
            string[] tags = { };
            this.api.Library_GetFileTags(file, this.fields, ref tags);
            return new LibraryTrackEx(tags);
        }
    }
}