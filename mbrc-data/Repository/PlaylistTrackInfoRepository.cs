﻿namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public class PlaylistTrackInfoRepository : IPlaylistTrackInfoRepository
    {
        public void DeletePlaylistTrackInfo(ICollection<PlaylistTrackInfo> tracks)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrackInfo> GetAllPlaylistTrackInfo()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrackInfo> GetCachedPlaylistTrackInfo()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrackInfo> GetDeletedPlaylistTrackInfo()
        {
            throw new System.NotImplementedException();
        }

        public PlaylistTrackInfo GetPlaylistTrackInfo(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetPlaylistTrackInfoCount()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrackInfo> GetPlaylistTrackInfoPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<PlaylistTrackInfo> GetUpdatedPlaylistTrackInfo(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public void SavePlaylistTrackInfo(PlaylistTrackInfo track)
        {
            throw new System.NotImplementedException();
        }

        public void SavePlaylistTrackInfo(ICollection<PlaylistTrackInfo> tracks)
        {
            throw new System.NotImplementedException();
        }
    }
}