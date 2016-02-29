namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistTrackRepository : GenericRepository<PlaylistTrack>, IPlaylistTrackRepository
    {
        public PlaylistTrackRepository(DatabaseProvider provider)
            : base(provider)
        {
        }

        public int GetTrackCountForPlaylist(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetTracksForPlaylist(long id)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetUpdatedTracksForPlaylist(int id, int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }
    }
}