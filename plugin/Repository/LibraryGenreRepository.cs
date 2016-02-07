namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using DapperExtensions;

    using MusicBeePlugin.AndroidRemote.Data;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    class LibraryGenreRepository : ILibraryGenreRepository
    {
        private CacheHelper cHelper;

        public LibraryGenre GetGenre(long id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveGenre(LibraryGenre genre)
        {
            throw new System.NotImplementedException();
        }

        public void SaveGenres(IEnumerable<LibraryGenre> genres)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryGenre> GetAllGenres()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryGenre> GetGenrePage(int offset, int limit)
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var page = limit == 0 ? 0 : offset / limit;
                var libraryGenres = connection.GetPage<LibraryGenre>(null, new List<ISort>(), page, limit);
                return libraryGenres;
            }
        }

        public IEnumerable<LibraryGenre> GetUpdatedGenres(int offset, int limit, long epoch)
        {
            using (var connection = this.cHelper.GetDbConnection())
            {
                var page = limit == 0 ? 0 : offset / limit;
                var group = new PredicateGroup { Operator = GroupOperator.Or, Predicates = new List<IPredicate>() };
                group.Predicates.Add(Predicates.Field<LibraryGenre>(genre => genre.DateAdded, Operator.Gt, epoch));
                group.Predicates.Add(Predicates.Field<LibraryGenre>(genre => genre.DateUpdated, Operator.Gt, epoch));
                group.Predicates.Add(Predicates.Field<LibraryGenre>(genre => genre.DateDeleted, Operator.Gt, epoch));
                var libraryGenres = connection.GetPage<LibraryGenre>(group, new List<ISort>(), page, limit);
                return libraryGenres;
            }
        }
    }
}