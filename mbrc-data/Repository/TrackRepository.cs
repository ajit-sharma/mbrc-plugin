namespace MusicBeeRemoteData.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;

    using Dapper;

    using MusicBeeRemoteData.Entities;

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

        public int DeleteTracks(ICollection<LibraryTrack> tracks)
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

        public ICollection<LibraryTrack> GetAllTracks()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<LibraryTrack>();
                connection.Close();
                return tracks.ToList();
            }
        }

        public ICollection<LibraryTrack> GetCachedTracks()
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryTrack> GetDeletedTracks()
        {
            throw new NotImplementedException();
        }

        public LibraryTrack GetTrack(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var track = connection.Get<LibraryTrack>(id);
                connection.Close();
                return track;
            }
        }

        public int GetTrackCount()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                var count = connection.RecordCount<LibraryTrack>(string.Empty);
                connection.Close();
                return count;
            }
        }

        public ICollection<LibraryTrack> GetTrackPage(int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryTrack> GetTracksByAlbumId(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.GetList<LibraryTrack>($"where AlbumId = {id}");
                connection.Close();
                return tracks.ToList();
            }
        }

        public ICollection<LibraryTrack> GetUpdatedTracks(int offset, int limit, long epoch)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var page = (limit == 0 || offset < limit) ? 1 : offset / limit;
                var paged = connection.GetListPaged<LibraryTrack>(page, limit, $"where DateUpdated > {epoch}", "Id asc");
                connection.Close();
                return paged.ToList();
            }
        }

        public int SaveTrack(LibraryTrack track)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var id = connection.Insert(track);
                connection.Close();
                return id ?? 0;
            }
        }

        public int SaveTracks(ICollection<LibraryTrack> tracks)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var rowsAffected = 0;
                tracks.ToObservable().Select(track => connection.Insert(track)).Subscribe(
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
    }
}