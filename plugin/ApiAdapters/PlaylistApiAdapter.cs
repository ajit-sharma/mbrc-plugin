namespace MusicBeeRemoteCore.ApiAdapters
{
    using System.Collections.Generic;
    using System.Linq;

    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteData.Entities;

    class PlaylistApiAdapter : IPlaylistApiAdapter
    {
        private readonly Plugin.MusicBeeApiInterface api;

        public PlaylistApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            this.api = api;
        }

        public bool AddTracks(string path, string[] list)
        {
            return this.api.Playlist_AppendFiles(path, list);
        }

        public string CreatePlaylist(string name, string[] list)
        {
            return this.api.Playlist_CreatePlaylist(string.Empty, name, list);
        }

        public bool DeletePlaylist(string path)
        {
            return this.api.Playlist_DeletePlaylist(path);
        }

        public List<Playlist> GetPlaylists()
        {
            this.api.Playlist_QueryPlaylists();
            var playlists = new List<Playlist>();
            while (true)
            {
                var path = this.api.Playlist_QueryGetNextPlaylist();
                var name = this.api.Playlist_GetName(path);
                string[] tracks = { };
                this.api.Playlist_QueryFilesEx(path, ref tracks);

                if (string.IsNullOrEmpty(path))
                {
                    break;
                }

                var playlist = new Playlist { Name = name, Path = path, Tracks = tracks.Count() };
                playlists.Add(playlist);
            }

            return playlists;
        }

        public List<PlaylistTrackInfo> GetPlaylistTracks(string path)
        {
            var list = new List<PlaylistTrackInfo>();
            var trackList = new string[] { };
            if (this.api.Playlist_QueryFilesEx(path, ref trackList))
            {
                var position = 0;
                list.AddRange(
                    trackList.Select(
                        trackPath =>
                        new PlaylistTrackInfo
                            {
                                Path = trackPath, 
                                Artist =
                                    this.api.Library_GetFileTag(trackPath, Plugin.MetaDataType.Artist), 
                                Title =
                                    this.api.Library_GetFileTag(
                                        trackPath, 
                                        Plugin.MetaDataType.TrackTitle), 
                                Position = position++
                            }));
            }

            return list;
        }

        public bool MoveTrack(string path, int @from, int to)
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

            return this.api.Playlist_MoveFiles(path, aFrom, dIn);
        }

        public bool PlayNow(string path)
        {
            return this.api.Playlist_PlayNow(path);
        }

        public bool RemoveTrack(string path, int position)
        {
            return this.api.Playlist_RemoveAt(path, position);
        }
    }
}