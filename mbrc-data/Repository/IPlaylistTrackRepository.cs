namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlaylistTrackRepository
    {
        void DeletePlaylistTracks(ICollection<PlaylistTrack> tracks);

        ICollection<PlaylistTrack> GetAllPlaylistTracks();

        ICollection<PlaylistTrack> GetCachedPlaylistTrackss();

        ICollection<PlaylistTrack> GetDeletedPlaylistTracks();

        PlaylistTrack GetPlaylistTrack(long id);

        int GetPlaylistTrackCount();

        ICollection<PlaylistTrack> GetPlaylistTrackPage(int offset, int limit);

        int GetTrackCountForPlaylist(int id);

        ICollection<PlaylistTrack> GetTracksForPlaylist(long id);

        ICollection<PlaylistTrack> GetUpdatedPlaylistTracks(int id, int offset, int limit, long epoch);

        void SavePlaylistTrack(PlaylistTrack track);

        void SavePlaylistTracks(ICollection<PlaylistTrack> tracks);
    }
}