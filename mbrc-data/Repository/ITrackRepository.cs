namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// The TrackRepository interface.
    /// </summary>
    public interface ITrackRepository
    {
        /// <summary>
        /// Deletes a list of tracks from the database and returns the number of rows affected.
        /// </summary>
        /// <param name="tracks">The tracks to be removed from the database.</param>
        /// <returns>The number of rows deleted.</returns>
        int DeleteTracks(ICollection<LibraryTrack> tracks);

        ICollection<LibraryTrack> GetAllTracks();

        ICollection<LibraryTrack> GetCachedTracks();

        ICollection<LibraryTrack> GetDeletedTracks();

        LibraryTrack GetTrack(long id);

        int GetTrackCount();

        ICollection<LibraryTrack> GetTrackPage(int offset, int limit);

        ICollection<LibraryTrack> GetTracksByAlbumId(long id);

        /// <summary>
        /// Gets a page of the tracks that where updated after the epoch supplied.
        /// </summary>
        /// <param name="offset">
        /// The offset of the data set.
        /// </param>
        /// <param name="limit">
        /// The number of data contained in the data set.
        /// </param>
        /// <param name="epoch">
        /// The epoch after when we want the data.
        /// </param>
        /// <returns>
        /// The <see cref="ICollection"/> of updated <see cref="LibraryTrack"/>s.
        /// </returns>
        ICollection<LibraryTrack> GetUpdatedTracks(int offset, int limit, long epoch);

        /// <summary>
        /// Saves a track to the backing database of the cache plugin.
        /// </summary>
        /// <param name="track">The track information</param>
        /// <returns>The id of the newly inserted track</returns>
        int SaveTrack(LibraryTrack track);

        /// <summary>
        /// Saves a list of tracks in the cache database of the plugin
        /// </summary>
        /// <param name="tracks">
        /// The track list.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> number of rows affected
        /// </returns>
        int SaveTracks(ICollection<LibraryTrack> tracks);
    }
}