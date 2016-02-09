namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlaylistTrackInfoRepository
    {
        void DeletePlaylistTrackInfo(ICollection<PlaylistTrackInfo> tracks);

        ICollection<PlaylistTrackInfo> GetAllPlaylistTrackInfo();

        ICollection<PlaylistTrackInfo> GetCachedPlaylistTrackInfo();

        ICollection<PlaylistTrackInfo> GetDeletedPlaylistTrackInfo();

        PlaylistTrackInfo GetPlaylistTrackInfo(long id);

        int GetPlaylistTrackInfoCount();

        ICollection<PlaylistTrackInfo> GetPlaylistTrackInfoPage(int offset, int limit);

        ICollection<PlaylistTrackInfo> GetUpdatedPlaylistTrackInfo(int offset, int limit, long epoch);

        void SavePlaylistTrackInfo(PlaylistTrackInfo track);

        void SavePlaylistTrackInfo(ICollection<PlaylistTrackInfo> tracks);
    }
}