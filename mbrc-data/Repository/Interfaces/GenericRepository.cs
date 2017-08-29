using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using MusicBeeRemoteData.Entities;
using MusicBeeRemoteData.Extensions;
using NLog;
using Logger = NLog.Logger;

namespace MusicBeeRemoteData.Repository.Interfaces
{
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
        protected static Logger _logger { get; } = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This constant defines the default limit of rows in a page in the case
        /// the received value is equal to zero.
        /// </summary>
        public const int DefaultLimit = 50;

        protected DatabaseProvider Provider;

        protected static string Table() => typeof(T).Name.ToLower();

        public GenericRepository(DatabaseProvider provider)
        {
            Provider = provider;
        }

        public int Delete(IList<T> t)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                var rowsAffected = 0;
                using (var transaction = db.BeginTrans())
                {
                    rowsAffected += t.Select(item => collection.Delete(item.Id)).Count(deleted => deleted);
                    transaction.Commit();
                }
                return rowsAffected;
            }
        }

        public IList<T> GetAll()
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return collection.FindAll().ToList();
            }
        }

        public T GetById(int id)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return collection.FindById(id);
            }
        }

        public IList<T> GetCached()
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return collection.Find(Query.EQ("DateDeleted", 0)).ToList();
            }
        }

        public int GetCount()
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return collection.Count();
            }
        }

        public IList<T> GetDeleted()
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return collection.Find(Query.GT("DateDeleted", 0)).ToList();
            }
        }

        public IList<T> GetPage(int offset, int limit)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                limit = Limit(limit);
                return collection.FindAll().Skip(offset).Take(limit).ToList();
            }
        }


        public IList<T> GetUpdatedPage(int offset, int limit, long epoch)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                limit = limit > 0 ? limit : DefaultLimit;
                var firstQuery = Query.Or(Query.GTE("DateUpdated", epoch), Query.GTE("DateAdded", epoch));
                var query = Query.Or(firstQuery, Query.GTE("DateDeleted", epoch));
                var updatedPage = collection.Find(query)
                    .Skip(offset)
                    .Take(limit)
                    .ToList();
                return updatedPage;
            }
        }

        public int Save(T t)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                if (t.Id <= 0)
                {
                    return collection.Insert(t);
                }
                t.DateUpdated = DateTime.Now.ToUnixTime();
                var updated = collection.Update(t);
                return updated ? t.Id : 0;
            }
        }

        public int Save(IList<T> t)
        {
            _logger.Debug($"Saving {t.Count} {typeof(T)} entries");

            if (t.Count == 0)
            {
                return 0;
            }

            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                var epoch = DateTime.UtcNow.ToUnixTime();
                var rowsAffected = 0;

                using (var transaction = db.BeginTrans())
                {
                    foreach (var item in t)
                    {
                        if (item.Id <= 0)
                        {
                            var id = collection.Insert(item);
                            if (id > 0)
                            {
                                rowsAffected++;
                            }
                        }
                        else
                        {
                            item.DateUpdated = epoch;
                            var updated = collection.Update(item);
                            if (updated)
                            {
                                rowsAffected++;
                            }
                        }
                    }
                    transaction.Commit();
                }
                return rowsAffected;
            }
        }

        public int SoftDelete(IList<T> elements)
        {
            if (elements.Count == 0)
            {
                return 0;
            }

            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                var epoch = DateTime.UtcNow.ToUnixTime();
                var rowsAffected = 0;
                using (var transaction = db.BeginTrans())
                {
                    foreach (var element in elements)
                    {
                        element.DateDeleted = epoch;
                        var updated = collection.Update(element);
                        if (updated)
                        {
                            rowsAffected++;
                        }
                    }
                    transaction.Commit();
                }


                return rowsAffected;
            }
        }

        protected void Execute(Action<LiteCollection<T>> action)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                action.Invoke(collection);
            }
        }

        protected IList<T> Execute(Func<LiteCollection<T>, IList<T>> function)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return function.Invoke(collection);
            }
        }

        protected T Execute(Func<LiteCollection<T>, T> function)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return function.Invoke(collection);
            }
        }

        protected int Execute(Func<LiteCollection<T>, int> function)
        {
            using (var db = new LiteDatabase(Provider.GetDatabaseFile()))
            {
                var collection = db.GetCollection<T>(Table());
                return function.Invoke(collection);
            }
        }

        protected static int Limit(int limit)
        {
            return limit > 0 ? limit : DefaultLimit;
        }
    }
}