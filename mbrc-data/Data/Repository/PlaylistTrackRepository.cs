using System.Collections.Generic;
using System.Linq;
using MusicBeeRemote.Data.Entities;
using MusicBeeRemote.Data.Repository.Interfaces;

namespace MusicBeeRemote.Data.Repository
{
    public class PlaylistTrackRepository : GenericRepository<PlaylistTrack>, IPlaylistTrackRepository
    {
        public PlaylistTrackRepository(DatabaseManager manager) : base(manager)
        {
        }

        public int GetTrackCountForPlaylist(int id)
        {
            return Execute(collection => collection.Count(track => track.PlaylistId == id));
        }

        public IList<PlaylistTrack> GetTracksForPlaylist(int id)
        {
            return Execute(collection => collection.Find(track => track.PlaylistId == id).ToList());
        }

        public IList<PlaylistTrack> GetUpdatedTracksForPlaylist(int id, int offset, int limit, long epoch)
        {
            return Execute(collection =>
            {
                limit = Limit(limit);

                return collection.Find(track =>
                        track.PlaylistId == id &&
                        track.DateAdded > epoch &&
                        track.DateDeleted > epoch &&
                        track.DateUpdated > epoch)
                    .Skip(offset)
                    .Take(limit)
                    .ToList();
            });
        }

        public void DeleteTracksForPlaylists(IList<int> deletedIds)
        {
            if (deletedIds.Count == 0)
            {
                return;
            }

            Execute(collection => collection.Delete(track => deletedIds.Contains(track.PlaylistId)));
        }

        public IList<int> GetUsedTrackInfoIds()
        {
            return Execute(collection => collection.FindAll().ToList()).Select(track => (int) track.TrackInfoId)
                .Where(id => id > 0)
                .ToList();
        }
    }
}