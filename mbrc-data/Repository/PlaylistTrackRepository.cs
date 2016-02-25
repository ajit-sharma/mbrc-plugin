namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public class PlaylistTrackRepository : IPlaylistTrackRepository
    {
        public void DeletePlaylistTracks(ICollection<PlaylistTrack> tracks)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrack> GetAllPlaylistTracks()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrack> GetCachedPlaylistTrackss()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrack> GetDeletedPlaylistTracks()
        {
            throw new System.NotImplementedException();
        }

        public PlaylistTrack GetPlaylistTrack(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetPlaylistTrackCount()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrack> GetPlaylistTrackPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public int GetTrackCountForPlaylist(int id)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrack> GetTracksForPlaylist(long id)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrack> GetUpdatedPlaylistTracks(int id, int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public void SavePlaylistTrack(PlaylistTrack track)
        {
            throw new System.NotImplementedException();
        }

        public void SavePlaylistTracks(ICollection<PlaylistTrack> tracks)
        {
            throw new System.NotImplementedException();
        }
    }
}