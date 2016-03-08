namespace MusicBeeRemoteData.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reactive.Linq;

    using Dapper;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Extensions;

    using NLog;

    /// <summary>
    /// The generic repository.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class GenericRepository<T> : IRepository<T>
        where T : TypeBase
    {
        /// <summary>
        /// The Logger instance for the current class.
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected DatabaseProvider provider;

        public GenericRepository(DatabaseProvider provider)
        {
            this.provider = provider;
        }

        public int Delete(IList<T> t)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var rowsAffected = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    t.ToObservable().ForEach(
                        track =>
                            {
                                var result = connection.Delete<T>(track.Id);
                                if (result > 0)
                                {
                                    rowsAffected++;
                                }
                            });
                    transaction.Commit();
                }

                connection.Close();
                return rowsAffected;
            }
        }

        public IList<T> GetAll()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<T>();
                connection.Close();
                return tracks.ToList();
            }
        }

        public T GetById(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var track = connection.Get<T>(id);
                connection.Close();
                return track;
            }
        }

        public IList<T> GetCached()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<T>("where DateDeleted = 0");
                connection.Close();
                return tracks.ToList();
            }
        }

        public int GetCount()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                var count = connection.RecordCount<T>(string.Empty);
                connection.Close();
                return count;
            }
        }

        public IList<T> GetDeleted()
        {
            {
                using (var connection = this.provider.GetDbConnection())
                {
                    connection.Open();
                    var tracks = connection.GetList<T>("where DateDeleted > 0");
                    connection.Close();
                    return tracks.ToList();
                }
            }
        }

        public IList<T> GetPage(int offset, int limit)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var page = (limit == 0) ? 1 : (offset / limit) + 1;
                var data = connection.GetListPaged<T>(page, limit, null, null);
                connection.Close();
                return data.ToList();
            }
        }

        public IList<T> GetUpdatedPage(int offset, int limit, long epoch)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var page = (limit == 0) ? 1 : (offset / limit) + 1;
                var paged = connection.GetListPaged<T>(
                    page, 
                    limit, 
                    $"where DateUpdated>={epoch} or DateAdded>={epoch} or DateDeleted>={epoch}", 
                    "Id asc");
                connection.Close();
                return paged.ToList();
            }
        }

        public int Save(T t)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var id = UpdateOrInsert(t, connection);
                connection.Close();
                return id ?? 0;
            }
        }

        public int Save(IList<T> t)
        {
            Logger.Debug($"Beginning operation (save) of {t.Count} files {t.GetType()}");
            var rowsAffected = 0;
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    rowsAffected += t.Select(a => UpdateOrInsert(a, connection, transaction))
                        .Count(result => result > 0);
                    transaction.Commit();
                }

                connection.Close();
            }

            return rowsAffected;
        }

        public int SoftDelete(IList<T> elements)
        {
            if (elements.Count == 0)
            {
                return 0;
            }

            using (var connection = this.provider.GetDbConnection())
            {
                var epoch = DateTime.UtcNow.ToUnixTime();
                var rowsAffected = 0;
                elements.ToObservable().ForEach(
                    t =>
                        {
                            t.DateDeleted = epoch;
                            var update = connection.Update(t);
                            if (update > 0)
                            {
                                rowsAffected++;
                            }
                        });

                return rowsAffected;
            }
        }

        private static int? UpdateOrInsert(T t, IDbConnection connection, IDbTransaction transaction = null)
        {
            if (t.Id <= 0)
            {
                return connection.Insert(t, transaction);
            }

            var epoch = DateTime.UtcNow.ToUnixTime();
            t.DateUpdated = epoch;
            var result = connection.Update(t, transaction);
            return (int?)(result > 0 ? t.Id : 0);
        }
    }
}