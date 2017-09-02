using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
  public class AlbumRepository : GenericRepository<AlbumDao>, IAlbumRepository
  {
    public AlbumRepository(DatabaseManager manager) : base(manager)
    {
    }
  }
}