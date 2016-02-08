using System.Collections.Generic;

namespace MusicBeePlugin.Repository
{
    using MusicBeePlugin.Rest.ServiceModel.Type;

    public interface ITrackRepository
    {
        LibraryTrack GetTrack(long id);

        void SaveTrack(LibraryTrack Track);

        void SaveTracks(ICollection<LibraryTrack> Tracks);

        ICollection<LibraryTrack> GetAllTracks();

        ICollection<LibraryTrack> GetTrackPage(int offset, int limit);

        ICollection<LibraryTrack> GetUpdatedTracks(int offset, int limit, long epoch);

        ICollection<LibraryTrack> GetCachedTracks();

        ICollection<LibraryTrack> GetDeletedTracks();

        void DeleteTracks(ICollection<LibraryTrack> Tracks);

        ICollection<LibraryTrack> GetTracksByAlbumId(long id);

        int GetTrackCount();
    }
}
