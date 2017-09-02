using System.Collections.Generic;
using System.Linq;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Data.Entities;

namespace MusicBeePlugin.ApiAdapters
{
    class PlaylistApiAdapter : IPlaylistApiAdapter
    {
        private readonly Plugin.MusicBeeApiInterface _api;

        public PlaylistApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            _api = api;
        }

        public bool AddTracks(string path, string[] list)
        {
            return _api.Playlist_AppendFiles(path, list);
        }

        public string CreatePlaylist(string name, string[] list)
        {
            return _api.Playlist_CreatePlaylist(string.Empty, name, list);
        }

        public bool DeletePlaylist(string path)
        {
            return _api.Playlist_DeletePlaylist(path);
        }

        public List<Playlist> GetPlaylists()
        {
            _api.Playlist_QueryPlaylists();
            var playlists = new List<Playlist>();
            while (true)
            {
                var path = _api.Playlist_QueryGetNextPlaylist();
                var name = _api.Playlist_GetName(path);
                string[] tracks;
                _api.Playlist_QueryFilesEx(path, out tracks);

                if (string.IsNullOrEmpty(path))
                {
                    break;
                }

                var playlist = new Playlist
                {
                    Name = name,
                    Url = path,
                    Tracks = tracks.Length
                };
                playlists.Add(playlist);
            }

            return playlists;
        }

        public List<PlaylistTrackInfo> GetPlaylistTracks(string path)
        {
            var list = new List<PlaylistTrackInfo>();
            string[] trackList;
            if (!_api.Playlist_QueryFilesEx(path, out trackList))
            {
                return list;
            }

            var position = 0;
            var tracks = trackList.Select(url => new PlaylistTrackInfo
            {
                Path = url,
                Artist = _api.Library_GetFileTag(url, Plugin.MetaDataType.Artist),
                Title = _api.Library_GetFileTag(url, Plugin.MetaDataType.TrackTitle),
                Position = position++
            });

            list.AddRange(tracks);

            return list;
        }

        public bool MoveTrack(string path, int from, int to)
        {
            int[] aFrom = {from};
            int dIn;
            if (from > to)
            {
                dIn = to - 1;
            }
            else
            {
                dIn = to;
            }

            return _api.Playlist_MoveFiles(path, aFrom, dIn);
        }

        public bool PlayNow(string path)
        {
            return _api.Playlist_PlayNow(path);
        }

        public bool RemoveTrack(string path, int position)
        {
            return _api.Playlist_RemoveAt(path, position);
        }
    }
}