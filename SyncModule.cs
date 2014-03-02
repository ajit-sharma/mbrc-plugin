using System.Globalization;

namespace MusicBeePlugin
{
    using System;
    using System.Collections.Generic;
    using AndroidRemote.Data;
    using AndroidRemote.Entities;
    using AndroidRemote.Networking;
    using AndroidRemote.Utilities;
    public class SyncModule : Messenger
    {
        
        private CacheHelper mHelper;
        private Plugin.MusicBeeApiInterface api;
        private List<LibraryData> mData;

        public SyncModule(Plugin.MusicBeeApiInterface api, String storagePath)
        {
            this.api = api;
            mHelper = new CacheHelper(storagePath);
        }

        public void SyncCheckForChanges(string[] cachedFiles ,DateTime lastSync)
        {
            string[] newFiles = {};
            string[] deletedFiles = {};
            string[] updatedFiles ={};

            api.Library_GetSyncDelta(cachedFiles, lastSync, Plugin.LibraryCategory.Music,
                ref newFiles, ref updatedFiles, ref deletedFiles);

            var jsonData = new
            {
                type = "partial",
                update = updatedFiles,
                deleted = deletedFiles,
                newfiles = newFiles
            };

            SendSocketMessage(Constants.Library, Constants.Reply, jsonData);
        }


        public void SyncGetCovers(string clientId, int offset, int limit)
        {
            var cached = mHelper.GetCoverHashes(limit, offset);
            var buffer = new List<ImageData>();
            foreach (var entry in cached)
            {
                var data = Utilities.GetCachedImage(entry.CoverHash);
                var image = new ImageData(entry.CoverHash, data) {album_id = entry.AlbumId};
                buffer.Add(image);
            }

            var pack = new
            {
                type = "cover",
                limit,
                offset,
                total = mHelper.GetCoversTotal(),
                data = buffer
            };

            SendSocketMessage(Constants.Library, Constants.Reply, pack, clientId);
        }

        /// <summary>
        /// Builds the cache. Creates an association of SHA1 hashes and file paths on the local filesystem.
        /// </summary>
        public void BuildCache()
        {
            string[] files = {};
            api.Library_QueryFilesEx(String.Empty, ref files);
            mHelper.CreateCache(files);
        }

        public void BuildCoverCache()
        {
            BuildCoverCachePerAlbum();
        }

        /// <summary>
        /// Builds the cover cache per album.
        /// This method is faster because it calls the GetArtworkUrl method for the first track of each album,
        /// however it might miss a number of covers;
        /// </summary>
        private void BuildCoverCachePerAlbum()
        {
            var total = mHelper.GetCachedFiles();
            var map = new Dictionary<string, AlbumEntry>();

            foreach (var libraryData in total)
            {
                var path = libraryData.Filepath;
                var id = api.Library_GetFileTag(path, Plugin.MetaDataType.AlbumId);
                AlbumEntry ab;
                if (!map.TryGetValue(id, out ab))
                {
                    ab = new AlbumEntry(id);
                    map.Add(id, ab);
                }
                var track_id = api.Library_GetFileTag(path, Plugin.MetaDataType.TrackNo);
                var track = new AlbumTrack(path, !string.IsNullOrEmpty(track_id) ? int.Parse(track_id, NumberStyles.Any) : 0);
                ab.Tracklist.Add(track);

            }

            var list = new List<AlbumEntry>(map.Values);

            foreach (var albumEntry in list)
            {
                albumEntry.Tracklist.Sort();
                var path = albumEntry.Tracklist[0].Path;
                var cover = api.Library_GetArtworkUrl(path, -1);
                if (string.IsNullOrEmpty(cover))
                {
                    continue;
                }

                albumEntry.CoverHash = Utilities.CacheArtworkImage(cover);
            }

            mHelper.BuildImageCache(list);
        }

        /// <summary>
        /// Builds the artist cover cache. 
        /// Method is really slow, due to multiple threads being called.
        /// Should be better called on a low priority thread.
        /// </summary>
        public void BuildArtistCoverCache()
        {
            List<Artist> artistList = new List<Artist>();
            if (api.Library_QueryLookupTable("artist", "count", ""))
            {
                foreach (string entry in api.Library_QueryGetLookupTableValue(null).Split(new[] {"\0\0"}, StringSplitOptions.None))
                {
                    string[] artistInfo = entry.Split(new[] { '\0' });
                    artistList.Add(new Artist(artistInfo[0], Int32.Parse(artistInfo[1])));
                }
            }

            api.Library_QueryLookupTable(null, null, null);
            foreach (var entry in artistList)
            {
                string[] urls = {};
                var artist = entry.artist;
                api.Library_GetArtistPictureUrls(artist, true, ref urls);
                if (urls.Length <= 0) continue;
                var hash = Utilities.CacheArtistImage(urls[0], artist);
                mHelper.CacheArtistUrl(artist, hash);
            }   
            
        }

        /// <summary>
        /// Used to get a batch of meta data from the api.
        /// </summary>
        /// <param name="offset">The offset number of the first result.</param>
        /// <param name="client">The id of the client.</param>
        /// <param name="limit">The limit.</param>
        public void SyncGetMetaData(int offset, string client, int limit = 50)
        {
            var cached = mHelper.GetCachedFiles();
            var count = cached.Count;
            
            var afterOffset = (count - offset);
            int internalLimit = limit;
            if (afterOffset - limit < 0)
            {
                internalLimit = afterOffset;
            }

            var buffer = new List<MetaData>();
            cached = cached.GetRange(offset, internalLimit);

            foreach (var data in cached)
            {
                var file = data.Filepath;
                var meta = new MetaData { hash = data.Hash, file = file };

                if (Plugin.MusicBeeVersion.v2_2 == api.MusicBeeVersion)
                {
                    meta.artist = api.Library_GetFileTag(file, Plugin.MetaDataType.Artist);
                    meta.album_artist = api.Library_GetFileTag(file, Plugin.MetaDataType.AlbumArtist);
                    meta.album = api.Library_GetFileTag(file, Plugin.MetaDataType.Album);
                    meta.title = api.Library_GetFileTag(file, Plugin.MetaDataType.TrackTitle);
                    meta.genre = api.Library_GetFileTag(file, Plugin.MetaDataType.Genre);
                    meta.year = api.Library_GetFileTag(file, Plugin.MetaDataType.Year);
                    meta.track_no = api.Library_GetFileTag(file, Plugin.MetaDataType.TrackNo);
                }
                else
                {
                    Plugin.MetaDataType[] types =
                    {
                        Plugin.MetaDataType.Artist,
                        Plugin.MetaDataType.AlbumArtist,
                        Plugin.MetaDataType.Album,
                        Plugin.MetaDataType.TrackTitle,
                        Plugin.MetaDataType.Genre,
                        Plugin.MetaDataType.Year,
                        Plugin.MetaDataType.TrackNo
                    };

                    var i = 0;
                    string[] tags = { };
                    api.Library_GetFileTags(file, types, ref tags);
                    meta.artist = tags[i++];
                    meta.album_artist = tags[i++];
                    meta.album = tags[i++];
                    meta.title = tags[i++];
                    meta.genre = tags[i++];
                    meta.year = tags[i++];
                    meta.track_no = tags[i];
                }

                buffer.Add(meta);
            }

            var pack = new
            {
                type = "meta",
                total = count,
                limit,
                offset,
                data = buffer
            };

            SendSocketMessage(Constants.Library, Constants.Reply, pack, client);
        }
    }
}