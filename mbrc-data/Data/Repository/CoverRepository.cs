using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
    public class CoverRepository : GenericRepository<LibraryCover>, ICoverRepository
    {
        public CoverRepository(DatabaseManager manager) : base(manager)
        {
        }
    }
}