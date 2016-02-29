namespace MusicBeeRemoteData.Repository
{
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistTrackInfoRepository : GenericRepository<PlaylistTrackInfo>, IPlaylistTrackInfoRepository
    {
        public PlaylistTrackInfoRepository(DatabaseProvider provider)
            : base(provider)
        {
        }
    }
}