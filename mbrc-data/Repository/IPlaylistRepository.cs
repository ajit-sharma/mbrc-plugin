namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public interface IPlaylistRepository
    {
        void DeletePlaylists(ICollection<Playlist> Playlists);

        ICollection<Playlist> GetAllPlaylists();

        ICollection<Playlist> GetCachedPlaylists();

        ICollection<Playlist> GetDeletedPlaylists();

        Playlist GetPlaylist(long id);

        int GetPlaylistCount();

        ICollection<Playlist> GetPlaylistPage(int offset, int limit);

        ICollection<Playlist> GetUpdatedPlaylists(int offset, int limit, long epoch);

        int SavePlaylist(Playlist Playlist);

        void SavePlaylists(ICollection<Playlist> Playlists);
    }
}