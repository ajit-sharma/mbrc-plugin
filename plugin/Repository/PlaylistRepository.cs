namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    class PlaylistRepository : IPlaylistRepository
    {
        public Playlist GetPlaylist(long id)
        {
            throw new System.NotImplementedException();
        }

        public int SavePlaylist(Playlist Playlist)
        {
            throw new System.NotImplementedException();
        }

        public void SavePlaylists(ICollection<Playlist> Playlists)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<Playlist> GetAllPlaylists()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<Playlist> GetPlaylistPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<Playlist> GetUpdatedPlaylists(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<Playlist> GetCachedPlaylists()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<Playlist> GetDeletedPlaylists()
        {
            throw new System.NotImplementedException();
        }

        public void DeletePlaylists(ICollection<Playlist> Playlists)
        {
            throw new System.NotImplementedException();
        }

        public int GetPlaylistCount()
        {
            throw new System.NotImplementedException();
        }
    }
}