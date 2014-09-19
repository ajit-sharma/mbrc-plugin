using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.OrmLite;
using Track = MusicBeePlugin.AndroidRemote.Entities.Track;

namespace MusicBeePlugin
{
    using AndroidRemote.Data;
    using AndroidRemote.Enumerations;
    using AndroidRemote.Networking;
    using AndroidRemote.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class PlaylistModule
    {
        private Plugin.MusicBeeApiInterface _api;
        private readonly CacheHelper _mHelper;

        public PlaylistModule(Plugin.MusicBeeApiInterface api, string mStoragePath)
        {
            _api = api;
            _mHelper = new CacheHelper(mStoragePath);
        }

        public void StoreAvailablePlaylists()
        {
            _api.Playlist_QueryPlaylists();
            var playlists = new List<Playlist>();
            while (true)
            {
                string[] files = { };
                var path = _api.Playlist_QueryGetNextPlaylist();
                if (String.IsNullOrEmpty(path)) break;
                var name = _api.Playlist_GetName(path);
                _api.Playlist_QueryFilesEx(path, ref files);
                var playlist = new Playlist(name, files.Count(), path);
                playlists.Add(playlist);
            }
            using (var db = _mHelper.GetDbConnection())
            {
                db.SaveAll(playlists);
            }

        }

        /// <summary>
        /// The function checks the MusicBee api and gets all the available playlist urls.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public void GetAvailablePlaylists(string client, int limit = 50, int offset = 0)
        {
            _api.Playlist_QueryPlaylists();
            var playlists = new List<Playlist>();
            
            while (true)
            {
                string[] files = { };
                var path = _api.Playlist_QueryGetNextPlaylist();
                if (String.IsNullOrEmpty(path)) break;
                var name = _api.Playlist_GetName(path);
                _api.Playlist_QueryFilesEx(path, ref files);
                var playlist = new Playlist(name, files.Count(), path);
                playlists.Add(playlist);
            }

            var count = playlists.Count;
            var afterOffset = (count - offset);
            var internalLimit = limit;
            if (afterOffset - limit < 0)
            {
                internalLimit = afterOffset;
            }

            playlists = playlists.GetRange(offset, internalLimit);

            var message = new
            {
                type = "get",
                limit,
                offset,
                total = count,
                playlists
            };
            
        }

        /// <summary>
        /// Given the url of a playlist and the id of a client the method sends a message to the specified client
        /// including the tracks in the specified playlist.
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
        /// Given the hash representing of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="path">The playlist path</param>
        public void RequestPlaylistPlayNow(string path)
        {
            //SendSocketMessage(Constants.PlaylistPlayNow, Constants.Reply, _api.Playlist_PlayNow(path));
        }

        /// <summary>
        /// Given the url of the playlist and the index of a index it removes the specified index,
        /// from the playlist.
        /// </summary>
        /// <param name="url">The url of th playlist</param>
        /// <param name="index">The index of the index to remove</param>
        public void RequestPlaylistTrackRemove(string url,int index)
        {
            var success = _api.Playlist_RemoveAt(url, index);
            //SendSocketMessage(Constants.PlaylistRemove, Constants.Reply, success);
        }

        public void RequestPlaylistCreate(string client, string name, MetaTag selection, string data)
        {
            var files = new string[] {};
            if (selection != MetaTag.track)
            {
                files = Plugin.Instance.GetUrlsForTag(selection, data);
            }
            var url = _api.Playlist_CreatePlaylist(String.Empty, name, files);
            //SendSocketMessage(Constants.PlaylistCreate, Constants.Reply, url);
        }

        public void RequestPlaylistMove(string clientId, string src, int from, int to)
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

            var success = _api.Playlist_MoveFiles(src, aFrom, dIn);

            var message = new
            {
                type = "move",
                success,
                @from,
                to
            };

            
        }

        /// <summary>
        /// Handles the request to add to tracks to an existing playlist
        /// </summary>
        /// <param name="client">The client id.</param>
        /// <param name="path">The path of the playlist.</param>
        /// <param name="selection">The selection type.</param>
        /// <param name="data">The data.</param>
        public void RequestPlaylistAddTracks(string client, string path, MetaTag selection, string data)
        {
            var files = new string[] {};
            if (selection != MetaTag.track)
            {
                files = Plugin.Instance.GetUrlsForTag(selection, data);
            }
            var success = _api.Playlist_AppendFiles(path, files);
            var message = new
            {
                type = "add",
                selection,
                data,
                success
            };
        }
    }
}
