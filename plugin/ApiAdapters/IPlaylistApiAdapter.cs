namespace MusicBeePlugin.ApiAdapters
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlaylistApiAdapter
    {
        string CreatePlaylist(string name, string[] list);

        List<PlaylistTrackInfo> GetPlaylistTracks(string path);

        bool RemoveTrack(string path, int position);

        bool MoveTrack(string path, int from, int to);

        bool AddTracks(string path, string[] list);

        bool DeletePlaylist(string path);

        bool PlayNow(string path);

        List<Playlist> GetPlaylists();
    }
}