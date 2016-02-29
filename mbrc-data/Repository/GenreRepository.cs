namespace MusicBeeRemoteData.Repository
{
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class GenreRepository : GenericRepository<LibraryGenre>, IGenreRepository
    {
        private DatabaseProvider provider;

        public GenreRepository(DatabaseProvider provider)
            : base(provider)
        {
            this.provider = provider;
        }
    }
}