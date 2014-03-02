using System;
using System.Collections.Generic;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.AndroidRemote.Networking;
using MusicBeePlugin.AndroidRemote.Utilities;

namespace MusicBeePlugin
{
    public class PlaylistModule : Messenger
    {
        private Plugin.MusicBeeApiInterface api;
        private String mStoragePath;

        public PlaylistModule(Plugin.MusicBeeApiInterface api, string mStoragePath)
        {
            this.api = api;
            this.mStoragePath = mStoragePath;
        }

        /// <summary>
        /// The function checks the MusicBee api and gets all the available playlist urls.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        public void GetAvailablePlaylists(string client, int limit = 50, int offset = 0)
        {
            api.Playlist_QueryPlaylists();
            string path;
            var playlists = new List<Playlist>();
            var ch = new CacheHelper(mStoragePath);
            while (true)
            {
                string[] files = { };
                path = api.Playlist_QueryGetNextPlaylist();
                if (String.IsNullOrEmpty(path)) break;
                var name = api.Playlist_GetName(path);
                api.Playlist_QueryFilesEx(path, ref files);
                var playlist = new Playlist(name, files.Count(), Utilities.Sha1Hash(path), path);
                playlists.Add(playlist);
                ch.CachePlaylists(playlists);
            }

            var count = playlists.Count;
            var afterOffset = (count - offset);
            int internalLimit = limit;
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
            var ch = new CacheHelper(mStoragePath);
            var playlist = ch.GetPlaylistByHash(hash);

            if (!api.Playlist_QueryFilesEx(playlist.path, ref pathList))
            {
                return;
            }

            var trackList = new List<Track>();
            var index = 0;

            foreach (var path in pathList)
            {
                var artist = api.Library_GetFileTag(path, Plugin.MetaDataType.Artist);
                var track = api.Library_GetFileTag(path, Plugin.MetaDataType.TrackTitle);
                var curTrack = new Track(artist, track, Utilities.Sha1Hash(path)) {index = ++index};
                trackList.Add(curTrack);
            }

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
        /// Given the url of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="url">The playlist url</param>
        public void RequestPlaylistPlayNow(string url)
        {
            SendSocketMessage(Constants.PlaylistPlayNow, Constants.Reply, api.Playlist_PlayNow(url));
        }

        /// <summary>
        /// Given the url of the playlist and the index of a index it removes the specified index,
        /// from the playlist.
        /// </summary>
        /// <param name="url">The url of th playlist</param>
        /// <param name="index">The index of the index to remove</param>
        public void RequestPlaylistTrackRemove(string url,int index)
        {
            bool success = api.Playlist_RemoveAt(url, index);
            SendSocketMessage(Constants.PlaylistRemove, Constants.Reply, success);
        }

        public void RequestPlaylistCreate(string client, string name, MetaTag selection, string data)
        {
            var files = new string[] {};
            if (selection != MetaTag.title)
            {
                files = Plugin.Instance.GetUrlsForTag(selection, data);
            }
            var url = api.Playlist_CreatePlaylist(String.Empty, name, files);
            SendSocketMessage(Constants.PlaylistCreate, Constants.Reply, url);
        }

        public void RequestPlaylistMove(string clientId, string src, int from, int to)
        {
            bool success;
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

            success = api.Playlist_MoveFiles(src, aFrom, dIn);

            var message = new
            {
                type = "move",
                success,
                @from,
                to
            };

            SendSocketMessage(Constants.Playlists, Constants.Reply, message, clientId);
        }
    }
}