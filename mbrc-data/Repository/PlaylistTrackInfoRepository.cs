using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace MusicBeeRemoteData.Repository
{
    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistTrackInfoRepository : GenericRepository<PlaylistTrackInfo>, IPlaylistTrackInfoRepository
    {
        public PlaylistTrackInfoRepository(DatabaseProvider provider)
            : base(provider)
        {
        }

      public IList<PlaylistTrackInfo> GetTrackForPlaylist(int id)
      {
        using (var connection = base.provider.GetDbConnection())
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
    }
}