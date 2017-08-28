namespace MusicBeeRemoteData.Repository.Interfaces
{
    using System.Collections.Generic;

    using Entities;

    /// <summary>
    /// The TrackRepository interface.
    /// </summary>
    public interface ITrackRepository : IRepository<LibraryTrack>
    {
        /// <summary>
        /// Gets the tracks for an album by the album id.
        /// </summary>
        /// <param name="id">
        /// The id of the album.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/> of tracks with the specified album id.
        /// </returns>
        IList<LibraryTrack> GetTracksByAlbumId(int id);

        string GetFirstAlbumTrackPathById(int id);
    }
}