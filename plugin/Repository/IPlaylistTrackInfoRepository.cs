
using System.Collections.Generic;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlaylistTrackInfoRepository
    {
        PlaylistTrackInfo GetPlaylistTrackInfo(long id);

        void SavePlaylistTrackInfo(PlaylistTrackInfo track);

        void SavePlaylistTrackInfo(ICollection<PlaylistTrackInfo> tracks);

        ICollection<PlaylistTrackInfo> GetAllPlaylistTrackInfo();

        ICollection<PlaylistTrackInfo> GetPlaylistTrackInfoPage(int offset, int limit);

        ICollection<PlaylistTrackInfo> GetUpdatedPlaylistTrackInfo(int offset, int limit, long epoch);

        ICollection<PlaylistTrackInfo> GetCachedPlaylistTrackInfo();

        ICollection<PlaylistTrackInfo> GetDeletedPlaylistTrackInfo();

        void DeletePlaylistTrackInfo(ICollection<PlaylistTrackInfo> tracks);

        int GetPlaylistTrackInfoCount();
    }
}
