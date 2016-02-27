namespace MusicBeeRemoteData.Repository
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    public interface ITrackRepository
    {
        void DeleteTracks(ICollection<LibraryTrack> Tracks);

        ICollection<LibraryTrack> GetAllTracks();

        ICollection<LibraryTrack> GetCachedTracks();

        ICollection<LibraryTrack> GetDeletedTracks();

        LibraryTrack GetTrack(long id);

        int GetTrackCount();

        ICollection<LibraryTrack> GetTrackPage(int offset, int limit);

        ICollection<LibraryTrack> GetTracksByAlbumId(long id);

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