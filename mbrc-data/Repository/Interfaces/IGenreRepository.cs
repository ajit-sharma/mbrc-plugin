namespace MusicBeeRemoteData.Repository.Interfaces
{
    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// The GenreRepository interface.
    /// The repository should contain only genre specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IGenreRepository : IRepository<LibraryGenre>
    {
    }
}