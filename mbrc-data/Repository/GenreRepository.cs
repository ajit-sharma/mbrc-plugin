namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using Dapper;

    using MusicBeeRemoteData;
    using MusicBeeRemoteData.Entities;

    public class GenreRepository : IGenreRepository
    {
        private DatabaseProvider cHelper;

        public void DeleteGenres(ICollection<LibraryGenre> genres)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryGenre> GetAllGenres()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryGenre> GetCachedGenres()
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var libraryGenres = connection.GetList<LibraryGenre>("where DateDeleted == 0");
                return libraryGenres.ToList();
            }
        }

        public ICollection<LibraryGenre> GetDeletedGenres()
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var libraryGenres = connection.GetList<LibraryGenre>("where DateDeleted > 0");
                return libraryGenres.ToList();
            }
        }

        public LibraryGenre GetGenre(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetGenreCount()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryGenre> GetGenrePage(int offset, int limit)
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var page = limit == 0 ? 0 : offset / limit;
                var libraryGenres = connection.GetListPaged<LibraryGenre>(page, limit, string.Empty, string.Empty);
                return libraryGenres.ToList();
            }
        }

        public ICollection<LibraryGenre> GetUpdatedGenres(int offset, int limit, long epoch)
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var page = limit == 0 ? 0 : offset / limit;

                var libraryGenres = connection.GetListPaged<LibraryGenre>(
                    page,
                    limit,
                    "where DateAdded > 0 or DateUpdated > 0 or DateDeleted > 0",
                    string.Empty);
                return libraryGenres.ToList();
            }
        }

        public void SaveGenre(LibraryGenre genre)
        {
            throw new System.NotImplementedException();
        }

        public void SaveGenres(ICollection<LibraryGenre> genres)
        {
            throw new System.NotImplementedException();
        }
    }
}