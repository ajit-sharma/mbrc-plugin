namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using DapperExtensions;

    using MusicBeePlugin.AndroidRemote.Data;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    class GenreRepository : IGenreRepository
    {
        private CacheHelper cHelper;

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
                var predicate = Predicates.Field<LibraryGenre>(genre => genre.DateDeleted, Operator.Eq, 0);
                var libraryGenres = connection.GetList<LibraryGenre>(predicate);
                return libraryGenres.ToList();
            }
        }

        public ICollection<LibraryGenre> GetDeletedGenres()
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var predicate = Predicates.Field<LibraryGenre>(genre => genre.DateDeleted, Operator.Gt, 0);
                var libraryGenres = connection.GetList<LibraryGenre>(predicate);
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
                var libraryGenres = connection.GetPage<LibraryGenre>(null, new List<ISort>(), page, limit);
                return libraryGenres.ToList();
            }
        }

        public ICollection<LibraryGenre> GetUpdatedGenres(int offset, int limit, long epoch)
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var page = limit == 0 ? 0 : offset / limit;
                var group = new PredicateGroup { Operator = GroupOperator.Or, Predicates = new List<IPredicate>() };
                group.Predicates.Add(Predicates.Field<LibraryGenre>(genre => genre.DateAdded, Operator.Gt, epoch));
                group.Predicates.Add(Predicates.Field<LibraryGenre>(genre => genre.DateUpdated, Operator.Gt, epoch));
                group.Predicates.Add(Predicates.Field<LibraryGenre>(genre => genre.DateDeleted, Operator.Gt, epoch));
                var libraryGenres = connection.GetPage<LibraryGenre>(group, new List<ISort>(), page, limit);
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