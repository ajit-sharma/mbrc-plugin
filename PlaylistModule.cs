using System.Diagnostics;

namespace MusicBeePlugin
{
    using AndroidRemote.Data;
    using AndroidRemote.Entities;
    using AndroidRemote.Enumerations;
    using AndroidRemote.Networking;
    using AndroidRemote.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class PlaylistModule : Messenger
    {
        private Plugin.MusicBeeApiInterface _api;
        private readonly String _mStoragePath;
        private readonly CacheHelper _mHelper;

        public PlaylistModule(Plugin.MusicBeeApiInterface api, string mStoragePath)
        {
            _api = api;
            _mStoragePath = mStoragePath;
            _mHelper = new CacheHelper(mStoragePath);
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
                var playlist = new Playlist(name, files.Count(), Utilities.Sha1Hash(path), path);
                playlists.Add(playlist);
                _mHelper.CachePlaylists(playlists);
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

            SendSocketMessage(Constants.Playlists, Constants.Reply, message, client);
        }

        /// <summary>
        /// Given the url of a playlist and the id of a client the method sends a message to the specified client
        /// including the tracks in the specified playlist.
        /// </summary>
        /// <param name="hash">sha1 hash identifying the playlist</param>
        /// <param name="clientId">The id of the client</param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public void GetTracksForPlaylist(string hash, string clientId, int limit = 50, int offset = 0)
        {

            string[] pathList = {};
            var ch = new CacheHelper(_mStoragePath);
            var playlist = ch.GetPlaylistByHash(hash);

            if (!_api.Playlist_QueryFilesEx(playlist.path, ref pathList))
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
                playlist_hash = hash,
                limit,
                offset,
                total = count,
                files = trackList
            };

            SendSocketMessage(Constants.Playlists, Constants.Reply, message, clientId);
        }

        /// <summary>
        /// Given the hash representing of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="hash">The playlist hash</param>
        public void RequestPlaylistPlayNow(string hash)
        {
            var url = _mHelper.GetPlaylistByHash(hash);
            SendSocketMessage(Constants.PlaylistPlayNow, Constants.Reply, _api.Playlist_PlayNow(url.path));
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
            SendSocketMessage(Constants.PlaylistRemove, Constants.Reply, success);
        }

        public void RequestPlaylistCreate(string client, string name, MetaTag selection, string data)
        {
            var files = new string[] {};
            if (selection != MetaTag.title)
            {
                files = Plugin.Instance.GetUrlsForTag(selection, data);
            }
            var url = _api.Playlist_CreatePlaylist(String.Empty, name, files);
            SendSocketMessage(Constants.PlaylistCreate, Constants.Reply, url);
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

            SendSocketMessage(Constants.Playlists, Constants.Reply, message, clientId);
        }

        /// <summary>
        /// Handles the request to add to tracks to an existing playlist
        /// </summary>
        /// <param name="client">The client id.</param>
        /// <param name="hash">The hash representing the playlist.</param>
        /// <param name="selection">The selection type.</param>
        /// <param name="data">The data.</param>
        public void RequestPlaylistAddTracks(string client, string hash, MetaTag selection, string data)
        {
            var files = new string[] {};
            var playlist = _mHelper.GetPlaylistByHash(hash);
            if (selection != MetaTag.title)
            {
                files = Plugin.Instance.GetUrlsForTag(selection, data);
            }
            var success = _api.Playlist_AppendFiles(playlist.path, files);
            var message = new
            {
                type = "add",
                hash,
                selection,
                data,
                success
            };
            SendSocketMessage(Constants.Playlists, Constants.Reply, message);
        }

        public void BuildPlaylistCache()
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
                var playlist = new Playlist(name, files.Count(), Utilities.Sha1Hash(path), path);
                playlists.Add(playlist);
                _mHelper.CachePlaylists(playlists);
            }
        }
    }
}