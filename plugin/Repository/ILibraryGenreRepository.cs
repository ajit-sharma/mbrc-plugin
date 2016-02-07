
namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    /// <summary>
    /// The LibraryGenreRepository interface.
    /// </summary>
    public interface ILibraryGenreRepository
    {
        /// <summary>
        /// Gets a specific genre from the repository by the genre id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="LibraryGenre"/>.
        /// </returns>
        LibraryGenre GetGenre(long id);

        void SaveGenre(LibraryGenre genre);

        void SaveGenres(IEnumerable<LibraryGenre> genres);

        IEnumerable<LibraryGenre> GetAllGenres();

        IEnumerable<LibraryGenre> GetGenrePage(int offset, int limit);

        IEnumerable<LibraryGenre> GetUpdatedGenres(int offset, int limit, long epoch);
    }
}
