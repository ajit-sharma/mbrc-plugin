namespace MusicBeeRemoteData.Repository
{
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class GenreRepository : GenericRepository<LibraryGenre>, IGenreRepository
    {
        public GenreRepository(DatabaseProvider provider) : base(provider)
        {

        }
    }
}