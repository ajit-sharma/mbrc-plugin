using System.Collections.Generic;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface ITrackRepository
    {
        LibraryTrack GetTrack(long id);

        void SaveTrack(LibraryTrack Track);

        void SaveTracks(IEnumerable<LibraryTrack> Tracks);

        IEnumerable<LibraryTrack> GetAllTracks();

        IEnumerable<LibraryTrack> GetTrackPage(int offset, int limit);

        IEnumerable<LibraryTrack> GetUpdatedTracks(int offset, int limit, long epoch);

        IEnumerable<LibraryTrack> GetCachedTracks();

        IEnumerable<LibraryTrack> GetDeletedTracks();

        void DeleteTracks(IEnumerable<LibraryTrack> Tracks);

        IEnumerable<LibraryTrack> GetTracksByAlbumId(long id);

        int GetTrackCount();
    }
}
