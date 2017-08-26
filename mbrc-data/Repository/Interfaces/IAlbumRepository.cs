namespace MusicBeeRemoteData.Repository.Interfaces
{
    using Entities;

    /// <summary>
    /// The AlbumRepository interface. 
    /// The repository should contain only album specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IAlbumRepository : IRepository<LibraryAlbum>
    {
    }
}