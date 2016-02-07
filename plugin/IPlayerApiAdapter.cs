namespace MusicBeePlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MusicBeePlugin.Model;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlayerApiAdapter
    {
        ICollection<LibraryAlbum> GetAlbumList();

        ICollection<LibraryArtist> GetArtistList();

        string GetArtistUrl(string name);

        byte[] GetCoverData(string path);

        string GetCoverUrl(string path);

        ICollection<LibraryGenre> GetGenreList();

        string[] GetLibraryFiles();

        Modifications GetSyncDelta(string[] cachedFiles, DateTime lastSync);

        LibraryTrackEx GetTags(string file);

        bool NowPlayingMoveTrack(int @from, int to);

        bool NowPlayingRemove(int index);

        bool QueueNow(string[] tracklist);

        bool QueueLast(string[] tracklist);

        bool QueueNext(string[] tracklist);

        bool PlayNow(string path);

        ICollection<NowPlaying> GetNowPlayingList();
    }

    class PlayerApiAdapter : IPlayerApiAdapter
    {
        public Plugin.MusicBeeApiInterface api;

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

        public PlayerApiAdapter(Plugin.MusicBeeApiInterface api)
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

        public bool NowPlayingMoveTrack(int @from, int to)
        {
            int[] aFrom = { @from };
            int dIn;
            if (@from > to)
            {
                dIn = to - 1;
            }
            else
            {
                dIn = to;
            }

            return this.api.NowPlayingList_MoveFiles(aFrom, dIn);
        }

        public bool NowPlayingRemove(int index)
        {
            return this.api.NowPlayingList_RemoveAt(index);
        }

        public bool QueueNow(string[] tracklist)
        {
            this.api.NowPlayingList_Clear();
            var success = this.api.NowPlayingList_QueueFilesNext(tracklist);

            if (tracklist != null && tracklist.Length > 0)
            {
                this.api.NowPlayingList_PlayNow(tracklist[0]);
            }
            return success;
        }

        public bool QueueLast(string[] tracklist)
        {
            return this.api.NowPlayingList_QueueFilesLast(tracklist);
        }

        public bool QueueNext(string[] tracklist)
        {
            return this.api.NowPlayingList_QueueFilesNext(tracklist);
        }

        public bool PlayNow(string path)
        {
            return this.api.NowPlayingList_PlayNow(path);
        }

        public ICollection<NowPlaying> GetNowPlayingList()
        {
            this.api.NowPlayingList_QueryFiles(null);

            var tracks = new List<NowPlaying>();
            var position = 1;

            while (true)
            {
                var playListTrack = this.api.NowPlayingList_QueryGetNextFile();
                if (string.IsNullOrEmpty(playListTrack))
                {
                    break;
                }

                var artist = this.api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.Artist);
                var title = this.api.Library_GetFileTag(playListTrack, Plugin.MetaDataType.TrackTitle);

                if (string.IsNullOrEmpty(artist))
                {
                    artist = "Unknown Artist";
                }

                if (string.IsNullOrEmpty(title))
                {
                    var index = playListTrack.LastIndexOf('\\');
                    title = playListTrack.Substring(index + 1);
                }

                var nowPlaying = new NowPlaying
                {
                    Artist = artist,
                    Id = position,
                    Path = playListTrack,
                    Position = position,
                    Title = title
                };

                tracks.Add(nowPlaying);
                position++;
            }

            return tracks;
        }
    }
}