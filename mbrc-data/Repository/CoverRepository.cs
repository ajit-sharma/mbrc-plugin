namespace MusicBeeRemoteData.Repository
{
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class CoverRepository : GenericRepository<LibraryCover>, ICoverRepository
    {
        public CoverRepository(DatabaseProvider provider) : base(provider)
        {
        }
    }
}