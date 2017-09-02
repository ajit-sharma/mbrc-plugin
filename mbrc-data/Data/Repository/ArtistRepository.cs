using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
    /// <summary>
    /// The artist repository.
    /// </summary>
    public class ArtistRepository : GenericRepository<ArtistDao>, IArtistRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistRepository"/> class.
        /// </summary>
        /// <param name="manager">
        /// The provider.
        /// </param>
        public ArtistRepository(DatabaseManager manager) : base(manager)
        {
        }
    }
}