namespace MusicBeeRemoteData.Repository.Interfaces
{
    using Entities;

    /// <summary>
    /// The CoverRepository interface.
    /// The repository should contain only cover specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface ICoverRepository : IRepository<LibraryCover>
    {
    }
}