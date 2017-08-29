namespace MusicBeeRemoteData.Repository
{
  using MusicBeeRemoteData.Entities;
  using Interfaces;

  public class AlbumRepository : GenericRepository<AlbumDao>, IAlbumRepository
  {
    public AlbumRepository(DatabaseProvider provider) : base(provider)
    {
    }
  }
}