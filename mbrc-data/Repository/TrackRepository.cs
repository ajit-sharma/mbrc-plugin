namespace MusicBeeRemoteData.Repository
{
    using System;
    using System.Collections.Generic;
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

        private readonly DatabaseProvider helper;

        public TrackRepository(DatabaseProvider helper)
        {
            this.helper = helper;
        }

        public void DeleteTracks(ICollection<LibraryTrack> Tracks)
        {
            throw new NotImplementedException();
        }

        public ICollection<LibraryTrack> GetAllTracks()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int GetTrackCount()
        {
            using (var connection = this.helper.GetDbConnection())
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
            throw new NotImplementedException();
        }

        public ICollection<LibraryTrack> GetUpdatedTracks(int offset, int limit, long epoch)
        {
            throw new NotImplementedException();
        }

        public int SaveTrack(LibraryTrack track)
        {
            using (var connection = this.helper.GetDbConnection())
            {
                connection.Open();
                var id = connection.Insert(track);
                connection.Close();
                return id ?? 0;
            }
        }

        public int SaveTracks(ICollection<LibraryTrack> tracks)
        {
            using (var connection = this.helper.GetDbConnection())
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