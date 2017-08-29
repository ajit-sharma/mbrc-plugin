using MusicBeeRemote.Core.ApiAdapters;

namespace plugin_tester
{
    using System;
    using System.Collections.Generic;
    using MusicBeeRemoteData.Entities;

    internal class PlaylistAdapter : IPlaylistApiAdapter
    {
        public bool AddTracks(string path, string[] list)
        {
            throw new NotImplementedException();
        }

        public string CreatePlaylist(string name, string[] list)
        {
            throw new NotImplementedException();
        }

        public bool DeletePlaylist(string path)
        {
            throw new NotImplementedException();
        }

        public List<Playlist> GetPlaylists()
        {
            throw new NotImplementedException();
        }

        public List<PlaylistTrackInfo> GetPlaylistTracks(string path)
        {
            throw new NotImplementedException();
        }

        public bool MoveTrack(string path, int @from, int to)
        {
            throw new NotImplementedException();
        }

        public bool PlayNow(string path)
        {
            throw new NotImplementedException();
        }

        public bool RemoveTrack(string path, int position)
        {
            throw new NotImplementedException();
        }
    }
}