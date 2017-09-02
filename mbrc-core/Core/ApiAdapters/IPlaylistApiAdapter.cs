using System.Collections.Generic;
using MusicBeeRemote.Data.Entities;

namespace MusicBeeRemote.Core.ApiAdapters
{
    public interface IPlaylistApiAdapter
    {
        bool AddTracks(string path, string[] list);

        string CreatePlaylist(string name, string[] list);

        bool DeletePlaylist(string path);

        List<Playlist> GetPlaylists();

        List<PlaylistTrackInfo> GetPlaylistTracks(string path);

        bool MoveTrack(string path, int from, int to);

        bool PlayNow(string path);

        bool RemoveTrack(string path, int position);
    }
}