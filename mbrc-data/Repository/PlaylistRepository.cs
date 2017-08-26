namespace MusicBeeRemoteData.Repository
{
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistRepository : GenericRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(DatabaseProvider provider) : base(provider)
        {
        }
    }
}