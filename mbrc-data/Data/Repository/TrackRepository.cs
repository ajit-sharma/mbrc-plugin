using System.Collections.Generic;
using System.Linq;
using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
    /// <summary>
    /// The track repository, gives access to all the track data in the plugin's cache.
    /// </summary>
    public class TrackRepository : GenericRepository<TrackDao>, ITrackRepository
    {
        public TrackRepository(DatabaseManager manager) : base(manager)
        {
        }

        public string GetFirstAlbumTrackPathById(int id)
        {
            return Execute(collection => collection.FindOne(track => track.AlbumId == id)).Path;
        }

        public IList<TrackDao> GetTracksByAlbumId(int id)
        {
            return Execute(collection => collection.Find(track => track.AlbumId == id).ToList());
        }
    }
}