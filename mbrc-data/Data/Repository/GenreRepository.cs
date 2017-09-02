using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
    public class GenreRepository : GenericRepository<GenreDao>, IGenreRepository
    {
        public GenreRepository(DatabaseManager manager) : base(manager)
        {

        }
    }
}