using System.Diagnostics;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.OrmLite;
using ServiceStack.Text;

namespace MusicBeePlugin
{
    using System.Linq;
    using System.Threading;
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using AndroidRemote.Data;
    using AndroidRemote.Entities;
    using AndroidRemote.Networking;
    using AndroidRemote.Utilities;
    /// <summary>
    /// Class SyncModule.
    /// Hosts the functionality responsible for the library sync operations.
    /// </summary>
    public class LibraryModule : Messenger
    {
        
        private readonly CacheHelper _mHelper;
        private Plugin.MusicBeeApiInterface _api;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryModule"/> class.
        /// </summary>
        /// <param name="api">The MusicBeeApiInterface instance</param>
        /// <param name="storagePath">The storage path used by MusicBee in the application data</param>
        public LibraryModule(Plugin.MusicBeeApiInterface api, String storagePath)
        {
            _api = api;
            _mHelper = new CacheHelper(storagePath);
        }

        /// <summary>
        /// Checks for changes in the library and updates the cache.
        /// </summary>
        /// <param name="cachedFiles">The cached files.</param>
        /// <param name="lastSync">The last synchronization date.</param>
        public void SyncCheckForChanges(string[] cachedFiles ,DateTime lastSync)
        {
            string[] newFiles = {};
            string[] deletedFiles = {};
            string[] updatedFiles ={};

            _api.Library_GetSyncDelta(cachedFiles, lastSync, Plugin.LibraryCategory.Music,
                ref newFiles, ref updatedFiles, ref deletedFiles);

            _mHelper.AddNewFilesToCache(newFiles);
            _mHelper.DeleteFilesFromCache(deletedFiles);
            _mHelper.MarkFilesUpdated(updatedFiles);
        }


        /// <summary>
        /// Sends a JSON message to the client containing the base64 encoded covers
        /// for the specified range.
        /// </summary>
        /// <param name="clientId">The identifier of the client that will receive the message.</param>
        /// <param name="offset">The offset represents the index of the first cover.</param>
        /// <param name="limit">The limit represents the number of the covers contained in the message.</param>
        public void SyncGetCovers(string clientId, int offset, int limit)
        {
//            var cached = _mHelper.GetCoverHashes(limit, offset);
//            var buffer = (from entry in cached
//                let data = Utilities.GetCachedCoverBase64(entry.CoverHash)
//                select new ImageData(entry.CoverHash, data) {album_id = entry.AlbumId}).ToList();
//
//            var pack = new
//            {
//                type = "cover",
//                limit,
//                offset,
//                total = _mHelper.GetCachedCoversCount(),
//                data = buffer
//            };
//
//            SendSocketMessage(Constants.Library, Constants.Reply, pack, clientId);
        }

        /// <summary>
        /// Builds the cache. Creates an association of SHA1 hashes and file paths on the local 
        /// filesystem and then updates the internal SQLite database.
        /// </summary>
        public void BuildCache()
        {
            string[] files = {};
            _api.Library_QueryFilesEx(String.Empty, ref files);
            using (var db = _mHelper.GetDbConnection())
            using (var trans = db.OpenTransaction())
            {
                db.SaveAll(GetAllArtists());
                db.SaveAll(GetAllGenres());
                db.SaveAll(GetAllAlbums());
                var artists = db.Select<LibraryArtist>();
                var genres = db.Select<LibraryGenre>();
                var albums = db.Select<LibraryAlbum>();
                LibraryGenre oGenre;
                LibraryArtist oArtist;
                LibraryArtist oAlbumArtist;
                LibraryAlbum oAlbum;
                foreach (var file in files)
                {
                    Plugin.MetaDataType[] types =
                    {
                        Plugin.MetaDataType.Artist,
                        Plugin.MetaDataType.AlbumArtist,
                        Plugin.MetaDataType.Album,
                        Plugin.MetaDataType.Genre,
                        Plugin.MetaDataType.TrackTitle,
                        Plugin.MetaDataType.Year,
                        Plugin.MetaDataType.TrackNo
                    };

                    var i = 0;
                    string[] tags = { };
                    _api.Library_GetFileTags(file, types, ref tags);

                    var artist = tags[i++];
                    var albumArtist = tags[i++];
                    var album = tags[i++];
                    var genre = tags[i++];
                    var title = tags[i++];
                    var year = tags[i++];
                    var trackNo = tags[i];
                                        
                    int iTrack;
                    int.TryParse(trackNo, out iTrack);

                    oGenre = genres.SingleOrDefault(q => q.name == genre);
                    oArtist = artists.SingleOrDefault(q => q.name == artist);
                    oAlbumArtist = artists.SingleOrDefault(q => q.name == albumArtist);
                    oAlbum = albums.SingleOrDefault(q => q.name == album);

                    if (oAlbum != null && oAlbumArtist != null)
                    {
                        oAlbum.artist_id = oAlbumArtist.id;
                    }

                    var track = new LibraryTrack
                    {
                        title = title,
                        year = year,
                        index = iTrack,
                        genre_id = oGenre != null ? oGenre.id : -1,
                        album_artist_id = oAlbumArtist != null ? oAlbumArtist.id : -1,
                        artist_id = oArtist !=null ? oArtist.id : -1,
                        album_id = oAlbum != null ? oAlbum.id : -1,
                        path = file
                    };
                    db.Save(track);
                }
                db.UpdateAll(albums);
                trans.Commit();
            }
        
        }

        private IEnumerable<LibraryArtist> GetAllArtists()
        {
            var list = new List<LibraryArtist>();
            if (_api.Library_QueryLookupTable("artist", "count", null))
            {
                list.AddRange(
                    _api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] {"\0\0"}, StringSplitOptions.None)
                        .Select(artist => new LibraryArtist(artist.Split(new[] {'\0'})[0])));
            }
            _api.Library_QueryLookupTable(null, null, null);
            return list;
        }

        private IEnumerable<LibraryGenre> GetAllGenres()
        {
            var list = new List<LibraryGenre>();
            if (_api.Library_QueryLookupTable("genre", "count", null))
            {
                list.AddRange(
                    _api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] {"\0\0"}, StringSplitOptions.None)
                        .Select(artist => new LibraryGenre(artist.Split(new[] {'\0'})[0])));
            }
            _api.Library_QueryLookupTable(null, null, null);
            return list;
        }

        private IEnumerable<LibraryAlbum> GetAllAlbums()
        {
            var list = new List<LibraryAlbum>();
            if (_api.Library_QueryLookupTable("album", "count", null))
            {
                list.AddRange(
                    _api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] { "\0\0" }, StringSplitOptions.None)
                        .Select(artist => new LibraryAlbum(artist.Split(new[] { '\0' })[0])));
            }
            _api.Library_QueryLookupTable(null, null, null);
            return list;
        } 

        /// <summary>
        /// Builds the cover cache per album.
        /// This method is faster because it calls the GetArtworkUrl method for the first track of each album,
        /// however it might miss a number of covers;
        /// </summary>
        private void BuildCoverCachePerAlbum()
        {
            using (var db = _mHelper.GetDbConnection())
            {
                var allTrack = db.Select<LibraryTrack>();
                var map = new Dictionary<string, LibraryAlbum>();
                var albums = db.Select<LibraryAlbum>();

                foreach (var lTrack in allTrack)
                {
                    var path = lTrack.path;
                    var id = _api.Library_GetFileTag(path, Plugin.MetaDataType.AlbumId);
                    LibraryAlbum ab;
                    if (!map.TryGetValue(id, out ab))
                    {
                        ab = albums.SingleOrDefault(q => q.id == lTrack.album_id) ?? new LibraryAlbum();
                        ab.album_id = id;
                        map.Add(id, ab);
                    }
                    var trackId = _api.Library_GetFileTag(path, Plugin.MetaDataType.TrackNo);
                    var track = new LibraryTrack
                    {
                        path = path,
                        index = !string.IsNullOrEmpty(trackId) ? int.Parse(trackId, NumberStyles.Any) : 0
                    };
                    ab.TrackList.Add(track);

                }

                var list = new List<LibraryAlbum>(map.Values);

                foreach (var albumEntry in list)
                {
                    albumEntry.TrackList.Sort();
                    var path = albumEntry.TrackList[0].path;
                    var cover = _api.Library_GetArtworkUrl(path, -1);
                    if (string.IsNullOrEmpty(cover))
                    {
                        continue;
                    }

                    albumEntry.cover_hash = Utilities.StoreCoverToCache(cover);
                }

                db.Update(list);
            }

        }

        /// <summary>
        /// Builds the artist cover cache. 
        /// Method is really slow, due to multiple threads being called.
        /// Should be better called on a low priority thread.
        /// </summary>
        public void BuildArtistCoverCache()
        {
            var artistList = new List<LibraryArtist>();
            if (_api.Library_QueryLookupTable("artist", "count", ""))
            {
                artistList.AddRange(
                    _api.Library_QueryGetLookupTableValue(null)
                        .Split(new[] {"\0\0"}, StringSplitOptions.None)
                        .Select(entry => entry.Split(new[] {'\0'}))
                        .Select(artistInfo => new LibraryArtist(artistInfo[0])));
            }

            _api.Library_QueryLookupTable(null, null, null);
            foreach (var entry in artistList)
            {
                string[] urls = {};
                var artist = entry.name;
                _api.Library_GetArtistPictureUrls(artist, true, ref urls);
                if (urls.Length <= 0) continue;
                var hash = Utilities.CacheArtistImage(urls[0], artist);
                _mHelper.CacheArtistUrl(artist, hash);
            }   
            
        }

        public LibraryTrack GetTrackById(int id)
        {
            using (var db = _mHelper.GetDbConnection())
            {
                return db.GetByIdOrDefault<LibraryTrack>(id);
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
            var cached = _mHelper.GetCachedFiles();
            var count = cached.Count;
            
            var afterOffset = (count - offset);
            var internalLimit = limit;
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

                if (Plugin.MusicBeeVersion.v2_2 == _api.MusicBeeVersion)
                {
                    meta.artist = _api.Library_GetFileTag(file, Plugin.MetaDataType.Artist);
                    meta.album_artist = _api.Library_GetFileTag(file, Plugin.MetaDataType.AlbumArtist);
                    meta.album = _api.Library_GetFileTag(file, Plugin.MetaDataType.Album);
                    meta.title = _api.Library_GetFileTag(file, Plugin.MetaDataType.TrackTitle);
                    meta.genre = _api.Library_GetFileTag(file, Plugin.MetaDataType.Genre);
                    meta.year = _api.Library_GetFileTag(file, Plugin.MetaDataType.Year);
                    meta.track_no = _api.Library_GetFileTag(file, Plugin.MetaDataType.TrackNo);
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
                    _api.Library_GetFileTags(file, types, ref tags);
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

        /// <summary>
        /// This method checks the state of the cache and is responsible for either
        /// building the cache when empty of updating on start.
        /// </summary>
        public void CheckCacheState()
        {
            var cached = _mHelper.GetCachedTracksCount();

            if (cached == 0)
            {
                Plugin.Instance.LibraryModule.BuildCache();
                var workerThread = new Thread(BuildCoverCachePerAlbum)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Normal
                };
                workerThread.Start();
            }
            else
            {
                var lastUpdate = _mHelper.GetLastMetaDataUpdate();
                var files = _mHelper.GetCachedFiles()
                    .Select(r => r.Filepath)
                    .ToArray();

                SyncCheckForChanges(files, lastUpdate);
            }
        }

        public List<LibraryTrack> GetAllTracks()
        {
            using (var db = _mHelper.GetDbConnection())
            {
                return db.Select<LibraryTrack>();
            }
        }
    }
}