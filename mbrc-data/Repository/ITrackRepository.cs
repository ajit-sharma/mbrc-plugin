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
}