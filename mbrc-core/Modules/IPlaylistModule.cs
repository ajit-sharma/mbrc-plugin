using MusicBeeRemoteCore.Rest.ServiceModel.Type;
using MusicBeeRemoteData.Entities;

namespace MusicBeeRemoteCore.Modules
{
    public interface IPlaylistModule
    {
        /// <summary>
        ///     Creates a new playlist.
        /// </summary>
        /// <param name="name">The name of the playlist that will be created.</param>
        /// <param name="list">
        ///     A string list containing the full paths of the tracks
        ///     that will be added to the playlist. If left empty an empty playlist will be created.
        /// </param>
        /// <returns>True if successfully completed and false otherwise.</returns>
        bool CreateNewPlaylist(string name, string[] list);

        /// <summary>
        ///     Removes a track from the specified playlist.
        /// </summary>
        /// <param name="id">The <c>id</c> of the playlist</param>
        /// <param name="position">The <c>position</c> of the track in the playlist</param>
        /// <returns></returns>
        bool DeleteTrackFromPlaylist(int id, int position);

        /// <summary>
        ///     Retrieves a page of <see cref="Playlist" /> results.
        /// </summary>
        /// <param name="limit">The number of entries in the page.</param>
        /// <param name="offset">The index of the first result in the page.</param>
        /// <param name="after">The date threshold after which the data are required</param>
        /// <returns>A PaginatedResponse containing playlists</returns>
        PaginatedResponse<Playlist> GetAvailablePlaylists(int limit = 50, int offset = 0, long after = 0);

        /// <summary>
        ///     Retrieves a page of <see cref="PlaylistTrack" /> results.
        /// </summary>
        /// <param name="id">The id of the playlist that contains the tracks.</param>
        /// <param name="limit">The number of the results in the page.</param>
        /// <param name="offset">The index of the first result in the page.</param>
        /// <param name="after"></param>
        /// <returns></returns>
        PaginatedResponse<PlaylistTrack> GetPlaylistTracks(
            int id, 
            int limit = 50, 
            int offset = 0, 
            long after = 0);

        /// <summary>
        ///     Retrieves a page of <see cref="PlaylistTrackInfo" /> results.
        /// </summary>
        /// <param name="limit">The number of the results in the page.</param>
        /// <param name="offset">The index of the first result in the page.</param>
        /// <param name="after"></param>
        /// <returns></returns>
        PaginatedResponse<PlaylistTrackInfo> GetPlaylistTracksInfo(
            int limit = 50, 
            int offset = 0, 
            long after = 0);

        /// <summary>
        ///     Moves a track in a playlist to a new position in the playlist.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <param name="from">The original position of the track in the playlist.</param>
        /// <param name="to">The new position of the track in the playlist.</param>
        /// <returns></returns>
        bool MovePlaylistTrack(int id, int from, int to);

        /// <summary>
        ///     Adds tracks to an existing playlist.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <param name="list">A list of the paths of the files in the filesystem.</param>
        /// <returns></returns>
        bool PlaylistAddTracks(int id, string[] list);

        /// <summary>
        ///     Deletes a playlist.
        /// </summary>
        /// <param name="id">The id of the playlist to delete.</param>
        /// <returns></returns>
        bool PlaylistDelete(int id);

        /// <summary>
        ///     Given the hash representing of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="path">The playlist path</param>
        ResponseBase PlaylistPlayNow(string path);

        /// <summary>
        ///     Syncs the playlist information in the cache with the information available
        ///     from the MusicBee API.
        /// </summary>
        /// <returns></returns>
        void SyncPlaylistsWithCache();

        /// <summary>
        ///     Syncs the <see cref="PlaylistTrack" /> cache with the data available
        ///     from the MusicBee API.
        /// </summary>
        /// <param name="playlist">The playlist for which the sync happens</param>
        bool SyncPlaylistDataWithCache(Playlist playlist);
    }
}