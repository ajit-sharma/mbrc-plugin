using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
    public class PlaylistRepository : GenericRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(DatabaseManager manager) : base(manager)
        {
        }
    }
}