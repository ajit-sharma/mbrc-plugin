namespace MusicBeeRemoteData.Repository.Interfaces
{
    using System.Collections.Generic;

    using Entities;

    /// <summary>
    /// The PlaylistTrackInfoRepository interface.
    /// The repository should contain only playlist track info specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IPlaylistTrackInfoRepository : IRepository<PlaylistTrackInfo>
    {
        /// <summary>
        /// Gets all the available
        /// </summary>
        /// <returns>A list of the available TrackInfo ids</returns>
        IList<int> GetAllIds();

        /// <summary>
        /// Gets a list of track info objects with position from the Playlist track table for the specified 
        /// Playlist tracks.
        /// </summary>
        /// <param name="id">The id of the playlist</param>
        /// <returns>A list of PlaylistTrackInfo</returns>
        IList<PlaylistTrackInfo> GetTracksForPlaylist(int id);

        /// <summary>
        /// Soft deletes the entries matching the supplied ids.
        /// </summary>
        /// <param name="unused">The ids that are not currently in use</param>
        /// <returns>The number of the rows affected.</returns>
        int SoftDeleteUnused(IList<int> unused);
    }
}