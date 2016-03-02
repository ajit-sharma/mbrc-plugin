namespace MusicBeeRemoteData.Repository
{
  using MusicBeeRemoteData.Entities;
  using Interfaces;

  public class AlbumRepository : GenericRepository<LibraryAlbum>, IAlbumRepository
  {
    public AlbumRepository(DatabaseProvider provider) : base(provider)
    {
    }
  }
}