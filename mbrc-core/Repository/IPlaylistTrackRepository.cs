
using System.Collections.Generic;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface IPlaylistTrackRepository
    {
        PlaylistTrack GetPlaylistTrack(long id);

        void SavePlaylistTrack(PlaylistTrack track);

        void SavePlaylistTracks(ICollection<PlaylistTrack> tracks);

        ICollection<PlaylistTrack> GetAllPlaylistTracks();

        ICollection<PlaylistTrack> GetPlaylistTrackPage(int offset, int limit);

        ICollection<PlaylistTrack> GetUpdatedPlaylistTracks(int id, int offset, int limit, long epoch);

        ICollection<PlaylistTrack> GetCachedPlaylistTrackss();

        ICollection<PlaylistTrack> GetDeletedPlaylistTracks();

        void DeletePlaylistTracks(ICollection<PlaylistTrack> tracks);

        int GetPlaylistTrackCount();

        ICollection<PlaylistTrack> GetTracksForPlaylist(long id);

        int GetTrackCountForPlaylist(int id);
    }
}