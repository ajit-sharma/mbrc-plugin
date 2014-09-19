#region

using System;
using System.Collections.Generic;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.AndroidRemote.Utilities;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using Track = MusicBeePlugin.AndroidRemote.Entities.Track;

#endregion

namespace MusicBeePlugin
{
    public class PlaylistModule
    {
        private readonly CacheHelper _mHelper;
        private Plugin.MusicBeeApiInterface _api;

        public PlaylistModule(Plugin.MusicBeeApiInterface api, string mStoragePath)
        {
            _api = api;
            _mHelper = new CacheHelper(mStoragePath);
        }

        public void StoreAvailablePlaylists()
        {
            var playlists = GetPlaylistsFromApi();
            string[] files = {};
            foreach (var playlist in playlists)
            {
                var path = playlist.Path;
                playlist.Name = _api.Playlist_GetName(path);
                _api.Playlist_QueryFilesEx(path, ref files);
                playlist.Tracks = files.Count();
            }

            using (var db = _mHelper.GetDbConnection())
            {
                db.SaveAll(GetNewPlaylists(playlists));
            }
        }

        private IEnumerable<Playlist> GetNewPlaylists(IEnumerable<Playlist> list)
        {
            var cachedLists = GetCachedPlaylists();
            var newPlaylists = list.Where(playlist =>
                !cachedLists.Exists(x =>
                    x.Path == playlist.Path))
                .ToList();
            return newPlaylists;
        }

        private List<Playlist> GetPlaylistsFromApi()
        {
            _api.Playlist_QueryPlaylists();
            var playlists = new List<Playlist>();
            while (true)
            {
                var path = _api.Playlist_QueryGetNextPlaylist();

                if (String.IsNullOrEmpty(path)) break;
                var playlist = new Playlist
                {
                    Path = path
                };
                playlists.Add(playlist);
            }
            return playlists;
        }

        private List<Playlist> GetCachedPlaylists()
        {
            using (var db = _mHelper.GetDbConnection())
            {
                return db.Select<Playlist>();
            }
        }

        /// <summary>
        ///     The function checks the MusicBee api and gets all the available playlist urls.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public PaginatedResponse GetAvailablePlaylists(int limit = 50, int offset = 0)
        {
            var playlists = GetCachedPlaylists();
            var total = playlists.Count;
            var result = new PaginatedResponse
            {
                Limit = limit,
                Offset = offset,
                Data = playlists,
                Total = total
            };
            return result;
        }

        public PaginatedResponse GetPlaylistTracks(int id, int limit = 50, int offset = 0)
        {
            using (var db = _mHelper.GetDbConnection())
            {
                string[] trackList = {};
                Playlist playlist;
                try
                {
                    playlist = db.GetById<Playlist>(id);
                }
                catch
                {
                    throw HttpError.NotFound("Playlist with id {0} does not exist".Fmt(id));
                }
                if (_api.Playlist_QueryFilesEx(playlist.Path, ref trackList))
                {
                    foreach (var track in trackList)
                    {
                    }
                }
            }

            return new PaginatedResponse
            {
                Limit = limit,
                Offset = offset,
                Total = 0
            };
        }

        /// <summary>
        ///     Given the url of a playlist and the id of a client the method sends a message to the specified client
        ///     including the tracks in the specified playlist.
        /// </summary>
        /// <param name="plPath"></param>
        /// <param name="clientId">The id of the client</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public void GetTracksForPlaylist(string plPath, string clientId, int limit = 50, int offset = 0)
        {
            string[] pathList = {};

            if (!_api.Playlist_QueryFilesEx(plPath, ref pathList))
            {
                return;
            }

            var index = 0;

            var trackList = (from path in pathList
                let artist = _api.Library_GetFileTag(path, Plugin.MetaDataType.Artist)
                let track = _api.Library_GetFileTag(path, Plugin.MetaDataType.TrackTitle)
                select new Track(artist, track, Utilities.Sha1Hash(path)) {index = ++index}).ToList();

            var count = trackList.Count;
            var afterOffset = (count - offset);
            var internalLimit = limit;
            if (afterOffset - limit < 0)
            {
                internalLimit = afterOffset;
            }

            trackList = trackList.GetRange(offset, internalLimit);

            var message = new
            {
                type = "gettracks",
                limit,
                offset,
                total = count,
                files = trackList
            };
        }

        /// <summary>
        ///     Given the hash representing of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="path">The playlist path</param>
        public SuccessResponse PlaylistPlayNow(string path)
        {
            return new SuccessResponse
            {
                success = _api.Playlist_PlayNow(path)
            };
        }

        /// <summary>
        ///     Given the url of the playlist and the index of a index it removes the specified index,
        ///     from the playlist.
        /// </summary>
        /// <param name="url">The url of th playlist</param>
        /// <param name="index">The index of the index to remove</param>
        public void RequestPlaylistTrackRemove(string url, int index)
        {
            var success = _api.Playlist_RemoveAt(url, index);
            //SendSocketMessage(Constants.PlaylistRemove, Constants.Reply, success);
        }

        public SuccessResponse RequestPlaylist(string name, string[] list)
        {
            var playlist = new Playlist
            {
                Name = name,
                Path = _api.Playlist_CreatePlaylist(String.Empty, name, list),
                Tracks = list.Count()
            };
            using (var db = _mHelper.GetDbConnection())
            {
                db.Save(playlist);
                return new SuccessResponse
                {
                    success = db.GetLastInsertId() > 0
                };
            }
        }

        public void RequestPlaylistMove(string clientId, string src, int from, int to)
        {
            int[] aFrom = {@from};
            int dIn;
            if (@from > to)
            {
                dIn = to - 1;
            }
            else
            {
                dIn = to;
            }

            var success = _api.Playlist_MoveFiles(src, aFrom, dIn);

            var message = new
            {
                type = "move",
                success,
                @from,
                to
            };
        }

        
        public bool PlaylistAddTracks(int id, string[] list)
        {
            using (var db = _mHelper.GetDbConnection())
            {
                var playlist = db.GetById<Playlist>(id);
                return _api.Playlist_AppendFiles(playlist.Path, list);
            }
        }
    }
}