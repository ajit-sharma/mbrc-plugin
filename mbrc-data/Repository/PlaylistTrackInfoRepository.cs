namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Dapper;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistTrackInfoRepository : GenericRepository<PlaylistTrackInfo>, IPlaylistTrackInfoRepository
    {
        public PlaylistTrackInfoRepository(DatabaseProvider provider)
            : base(provider)
        {
        }

        public IList<int> GetAllIds()
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var ids = connection.Query<int>(@"select Id from PlaylistTrackInfo");
                connection.Close();
                return ids.ToList();
            }
        }

        public IList<PlaylistTrackInfo> GetTrackForPlaylist(int id)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                connection.Open();
                var tracks = connection.Query<PlaylistTrackInfo>(@"select * from PlaylistTrackInfo as pi 
                                                                      inner join PlaylistTrack as tr 
                                                                      on pi.Id = tr.TrackInfoId
                                                                      order by tr.Position asc");
                connection.Close();
                return tracks.ToList();
            }
        }

        public int SoftDeleteUnused(IList<int> unused)
        {
            using (var connection = this.provider.GetDbConnection())
            {
                Debug.WriteLine(string.Join(",", unused));
                return connection.DeleteList<PlaylistTrack>($"where PlaylistId in ({string.Join(",", unused)})");
            }
        }
    }
}