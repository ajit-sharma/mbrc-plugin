namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    /// <summary>
    /// The track repository, gives access to all the track data in the plugin's cache.
    /// </summary>
    public class TrackRepository : GenericRepository<LibraryTrack>, ITrackRepository
    {
        public TrackRepository(DatabaseProvider provider) : base(provider)
        {
        }

        public string GetFirstAlbumTrackPathById(int id)
        {
            return Execute(collection => collection.FindOne(track => track.AlbumId == id)).Path;
        }

        public IList<LibraryTrack> GetTracksByAlbumId(int id)
        {
            return Execute(collection => collection.Find(track => track.AlbumId == id).ToList());
        }
    }
}