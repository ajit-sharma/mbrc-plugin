namespace MusicBeeRemoteData.Repository
{    
    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// The ArtistRepository interface.
    /// The repository should contain only artist specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IArtistRepository : IRepository<LibraryArtist>
    {
    }
}