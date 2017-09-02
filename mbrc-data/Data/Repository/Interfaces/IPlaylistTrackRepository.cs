using System.Collections.Generic;
using MusicBeeRemote.Data.Entities;

namespace MusicBeeRemote.Data.Repository.Interfaces

{
    /// <summary>
    /// The PlaylistTrackRepository interface.
    /// The repository should contain only playlist track specific repository methods.
    /// The generic repository methods should be in the <see cref="IRepository{T}"/>.
    /// </summary>
    public interface IPlaylistTrackRepository : IRepository<PlaylistTrack>
    {
        /// <summary>
        /// Gets the cached track count for a playlist. 
        /// </summary>
        /// <param name="id">
        /// The id of the playlist
        /// </param>
        /// <returns>
        /// The <see cref="int"/>. The number of cached entries for the Playlist
        /// </returns>
        int GetTrackCountForPlaylist(int id);

        /// <summary>
        /// Gets the cached tracks for a playlist by the the playlist id.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <returns>The list of tracks in the playlist</returns>
        IList<PlaylistTrack> GetTracksForPlaylist(int id);

        /// <summary>
        /// Gets a page of updated tracks for the specified playlist.. 
        /// </summary>
        /// <param name="id">The id of the playlist</param>
        /// <param name="offset">The offset of the first element.</param>
        /// <param name="limit">The number of data rows in the data set</param>
        /// <param name="epoch">The unix epoch threshold. Data updated after this point should be in the data set</param>
        /// <returns>A page (list) of playlist tracks.</returns>
        IList<PlaylistTrack> GetUpdatedTracksForPlaylist(int id, int offset, int limit, long epoch);

        /// <summary>
        /// Soft deletes the playlist track entries that match the provided playlist ids.
        /// </summary>
        /// <param name="deletedIds">The ids of the tracks to soft delete.</param>
        void DeleteTracksForPlaylists(IList<int> deletedIds);


        /// <summary>
        /// Gets all the track info ids used in tracks
        /// </summary>
        IList<int> GetUsedTrackInfoIds();
    }
}