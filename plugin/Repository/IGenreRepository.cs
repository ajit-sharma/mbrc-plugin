
namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    /// <summary>
    /// The GenreRepository interface.
    /// </summary>
    public interface IGenreRepository
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

        IEnumerable<LibraryGenre> GetCachedGenres();

        IEnumerable<LibraryGenre> GetDeletedGenres();

        void DeleteGenres(IEnumerable<LibraryGenre> genres);

        int GetGenreCount();
    }
}
