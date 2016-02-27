namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// The TrackRepository interface.
    /// </summary>
    public interface ITrackRepository : IRepository<LibraryTrack>
    {
        IList<LibraryTrack> GetTracksByAlbumId(long id);
    }

    /// <summary>
    /// The interface provides a base for the basic actions of a repository.
    /// </summary>
    /// <typeparam name="T">The type of data the repository handles</typeparam>
    public interface IRepository<T>
    {
        int Delete(IList<T> t);
                
        int SoftDelete(IList<T> t);

        IList<T> GetAll();

        IList<T> GetCached();

        IList<T> GetDeleted();

        T GetById(long id);

        int GetCount();

        IList<T> GetPage(int offset, int limit);
        
        IList<T> GetUpdatedPage(int offset, int limit, long epoch);
        
        int Save(T t);
        
        int Save(IList<T> t);
    }
}