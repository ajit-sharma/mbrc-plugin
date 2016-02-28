namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistTrackInfoRepository : IPlaylistTrackInfoRepository
    {
        public int Delete(IList<PlaylistTrackInfo> tracks)
        {
            throw new System.NotImplementedException();
        }

        public int SoftDelete(IList<PlaylistTrackInfo> t)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrackInfo> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrackInfo> GetCached()
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrackInfo> GetDeleted()
        {
            throw new System.NotImplementedException();
        }

        public PlaylistTrackInfo GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetCount()
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrackInfo> GetPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IList<PlaylistTrackInfo> GetUpdatedPage(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public int Save(PlaylistTrackInfo track)
        {
            throw new System.NotImplementedException();
        }

        public int Save(IList<PlaylistTrackInfo> tracks)
        {
            throw new System.NotImplementedException();
        }
    }
}