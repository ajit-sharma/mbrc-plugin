namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    class TrackRepository : ITrackRepository
    {
        public LibraryTrack GetTrack(long id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveTrack(LibraryTrack Track)
        {
            throw new System.NotImplementedException();
        }

        public void SaveTracks(IEnumerable<LibraryTrack> Tracks)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryTrack> GetAllTracks()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryTrack> GetTrackPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryTrack> GetUpdatedTracks(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryTrack> GetCachedTracks()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryTrack> GetDeletedTracks()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteTracks(IEnumerable<LibraryTrack> Tracks)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<LibraryTrack> GetTracksByAlbumId(long id)
        {
            throw new System.NotImplementedException();
        } 
    }
}