namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Dapper;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistTrackRepository : GenericRepository<PlaylistTrack>, IPlaylistTrackRepository
    {
        public PlaylistTrackRepository(DatabaseProvider provider)
            : base(provider)
        {
        }

        public int GetTrackCountForPlaylist(int id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                return connection.RecordCount<PlaylistTrack>($"where PlaylistId={id}");
            }
        }

        public IList<PlaylistTrack> GetTracksForPlaylist(long id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                return connection.GetList<PlaylistTrack>($"where PlaylistId={id}").ToList();
            }
        }

        public IList<PlaylistTrack> GetUpdatedTracksForPlaylist(int id, int offset, int limit, long epoch)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                var page = (limit == 0) ? 1 : (offset / limit) + 1;
                var select =
                    $"where PlaylistId={id} and (DateUpdated > {epoch} or DateDeleted > {epoch} or DateAdded > {epoch}";
                return connection.GetListPaged<PlaylistTrack>(page, limit, select, "Id asc").ToList();
            }
        }

        public void DeleteTracksForPlaylists(IList<long> deletedIds)
        {
            if (deletedIds.Count == 0)
            {
                return;
            }

            using (var connection = this.provider.GetDbConnection())
            {
                Debug.WriteLine(string.Join(",", deletedIds));
                connection.DeleteList<PlaylistTrack>($"where PlaylistId in ({string.Join(",", deletedIds)})");
            }
        }
    }
}