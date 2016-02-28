namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;
    using MusicBeeRemoteData.Repository.Interfaces;

    public class PlaylistRepository : IPlaylistRepository
    {
        public int Delete(IList<Playlist> Playlists)
        {
            throw new System.NotImplementedException();
        }

        public int SoftDelete(IList<Playlist> t)
        {
            throw new System.NotImplementedException();
        }

        public IList<Playlist> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<Playlist> GetCached()
        {
            throw new System.NotImplementedException();
        }

        public IList<Playlist> GetDeleted()
        {
            throw new System.NotImplementedException();
        }

        public Playlist GetById(long id)
        {
            throw new System.NotImplementedException();
        }

        public int GetCount()
        {
            throw new System.NotImplementedException();
        }

        public IList<Playlist> GetPage(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public IList<Playlist> GetUpdatedPage(int offset, int limit, long epoch)
        {
            throw new System.NotImplementedException();
        }

        public int Save(Playlist Playlist)
        {
            throw new System.NotImplementedException();
        }

        public int Save(IList<Playlist> Playlists)
        {
            throw new System.NotImplementedException();
        }
    }
}