namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using Dapper;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class GenreRepository : IGenreRepository
    {
        private DatabaseProvider cHelper;

        public int Delete(IList<LibraryGenre> genres)
        {
            throw new System.NotImplementedException();
        }

        public int SoftDelete(IList<LibraryGenre> t)
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryGenre> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryGenre> GetCached()
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var libraryGenres = connection.GetList<LibraryGenre>("where DateDeleted == 0");
                return libraryGenres.ToList();
            }
        }

        public IList<LibraryGenre> GetDeleted()
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var libraryGenres = connection.GetList<LibraryGenre>("where DateDeleted > 0");
                return libraryGenres.ToList();
            }
        }

        public LibraryGenre GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetCount()
        {
            throw new System.NotImplementedException();
        }

        public IList<LibraryGenre> GetPage(int offset, int limit)
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var page = limit == 0 ? 0 : offset / limit;
                var libraryGenres = connection.GetListPaged<LibraryGenre>(page, limit, string.Empty, string.Empty);
                return libraryGenres.ToList();
            }
        }

        public IList<LibraryGenre> GetUpdatedPage(int offset, int limit, long epoch)
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

        public int Save(LibraryGenre genre)
        {
            throw new System.NotImplementedException();
        }

        public int Save(IList<LibraryGenre> genres)
        {
            throw new System.NotImplementedException();
        }
    }
}