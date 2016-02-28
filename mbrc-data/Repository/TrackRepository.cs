namespace MusicBeeRemoteData.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reactive.Linq;

    using Dapper;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Extensions;
    using MusicBeeRemoteData.Repository.Interfaces;

    using NLog;

    /// <summary>
    /// The track repository, gives access to all the track data in the plugin's cache.
    /// </summary>
    public class TrackRepository : ITrackRepository
    {
        /// <summary>
        /// The Logger instance for the current class.
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly DatabaseProvider provider;

        public TrackRepository(DatabaseProvider provider)
        {
            this.provider = provider;
        }

        public int Delete(IList<LibraryTrack> tracks)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var rowsAffected = 0;
                using (var transaction = connection.BeginTransaction())
                {
                    tracks.ToObservable().ForEach(
                        track =>
                            {
                                var result = connection.Delete<LibraryTrack>(track.Id);
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

        public IList<LibraryTrack> GetAll()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<LibraryTrack>();
                connection.Close();
                return tracks.ToList();
            }
        }

        public LibraryTrack GetById(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var track = connection.Get<LibraryTrack>(id);
                connection.Close();
                return track;
            }
        }

        public IList<LibraryTrack> GetCached()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<LibraryTrack>("where DateDeleted = 0");
                connection.Close();
                return tracks.ToList();
            }
        }

        public int GetCount()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                var count = connection.RecordCount<LibraryTrack>(string.Empty);
                connection.Close();
                return count;
            }
        }

        public IList<LibraryTrack> GetDeleted()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<LibraryTrack>("where DateDeleted > 0");
                connection.Close();
                return tracks.ToList();
            }
        }

        public IList<LibraryTrack> GetPage(int offset, int limit)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var page = (limit == 0) ? 1 : (offset / limit) + 1;
                var data = connection.GetListPaged<LibraryTrack>(page, limit, null, null);
                connection.Close();
                return data.ToList();
            }
        }

        public IList<LibraryTrack> GetTracksByAlbumId(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<LibraryTrack>($"where AlbumId = {id}");
                connection.Close();
                return tracks.ToList();
            }
        }

        public IList<LibraryTrack> GetUpdatedPage(int offset, int limit, long epoch)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var page = (limit == 0) ? 1 : (offset / limit) + 1;
                var paged = connection.GetListPaged<LibraryTrack>(
                    page, 
                    limit, 
                    $"where DateUpdated>={epoch} or DateAdded>={epoch} or DateDeleted>={epoch}", 
                    "Id asc");
                connection.Close();
                return paged.ToList();
            }
        }

        public int Save(LibraryTrack track)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var id = UpdateOrInsert(track, connection);       
                connection.Close();
                return id ?? 0;
            }
        }

        public int Save(IList<LibraryTrack> tracks)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var rowsAffected = 0;
                tracks.ToObservable()
                    .Select(track => UpdateOrInsert(track, connection))
                    .Subscribe(
                        id =>
                            {
                                if (id > 0)
                                {
                                    rowsAffected++;
                                }
                            }, 
                        exception => { Logger.Debug(exception, "failed to insert the tracks"); });

                connection.Close();
                return rowsAffected;
            }
        }

        private static int? UpdateOrInsert(LibraryTrack track, IDbConnection connection)
        {
            if (track.Id <= 0)
            {
                return connection.Insert(track);
            }

            var epoch = DateTime.UtcNow.ToUnixTime();
            track.DateUpdated = epoch;
            var result = connection.Update(track);
            return (int?)(result > 0 ? track.Id : 0);
        }

        public int SoftDelete(IList<LibraryTrack> tracks)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                var epoch = DateTime.UtcNow.ToUnixTime();
                var rowsAffected = 0;
                tracks.ToObservable().ForEach(
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
    }
}