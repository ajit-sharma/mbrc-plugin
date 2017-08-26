namespace MusicBeeRemoteData.Repository
{
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    /// <summary>
    /// The artist repository.
    /// </summary>
    public class ArtistRepository : GenericRepository<LibraryArtist>, IArtistRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistRepository"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public ArtistRepository(DatabaseProvider provider) : base(provider)
        {
        }
    }
}