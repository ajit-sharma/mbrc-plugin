namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistTrackRepository : IPlaylistTrackRepository
    {
        public int Delete(IList<PlaylistTrack> t)
        {
            throw new System.NotImplementedException();
        }

        public int SoftDelete(IList<PlaylistTrack> t)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetCached()
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetDeleted()
        {
            throw new System.NotImplementedException();
        }

        public PlaylistTrack GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetCount()
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetUpdatedPage(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public int Save(PlaylistTrack t)
        {
            throw new System.NotImplementedException();
        }

        public int Save(IList<PlaylistTrack> t)
        {
            throw new System.NotImplementedException();
        }

        public int GetTrackCountForPlaylist(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetTracksForPlaylist(long id)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrack> GetUpdatedTracksForPlaylist(int id, int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }
    }
}