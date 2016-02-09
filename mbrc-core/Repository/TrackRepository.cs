namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using DapperExtensions;

    using MusicBeePlugin.AndroidRemote.Data;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    class TrackRepository : ITrackRepository
    {
        private readonly CacheHelper helper;

        public TrackRepository(CacheHelper helper)
        {
            this.helper = helper;
        }

        public void DeleteTracks(ICollection<LibraryTrack> Tracks)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryTrack> GetAllTracks()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryTrack> GetCachedTracks()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryTrack> GetDeletedTracks()
        {
            throw new System.NotImplementedException();
        }

        public LibraryTrack GetTrack(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetTrackCount()
        {
            using (var connection = this.helper.GetDbConnection())
            {
                var count = connection.Count<LibraryTrack>(null);
                connection.Close();
                return count;
            }
        }

        public ICollection<LibraryTrack> GetTrackPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryTrack> GetTracksByAlbumId(long id)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<LibraryTrack> GetUpdatedTracks(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public void SaveTrack(LibraryTrack Track)
        {
            throw new System.NotImplementedException();
        }

        public void SaveTracks(ICollection<LibraryTrack> Tracks)
        {
            throw new System.NotImplementedException();
        }
    }
}