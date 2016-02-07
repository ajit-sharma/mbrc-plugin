
using System.Collections.Generic;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlaylistRepository
    {
        Playlist GetPlaylist(long id);

        int SavePlaylist(Playlist Playlist);

        void SavePlaylists(ICollection<Playlist> Playlists);

        ICollection<Playlist> GetAllPlaylists();

        ICollection<Playlist> GetPlaylistPage(int offset, int limit);

        ICollection<Playlist> GetUpdatedPlaylists(int offset, int limit, long epoch);

        ICollection<Playlist> GetCachedPlaylists();

        ICollection<Playlist> GetDeletedPlaylists();

        void DeletePlaylists(ICollection<Playlist> Playlists);

        int GetPlaylistCount();
    }
}
