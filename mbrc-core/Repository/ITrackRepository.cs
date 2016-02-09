namespace MusicBeePlugin.Repository
{
    using System.Collections.Generic;

    using MusicBeePlugin.Rest.ServiceModel.Type;

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

        void SaveTrack(LibraryTrack Track);

        void SaveTracks(ICollection<LibraryTrack> Tracks);
    }
}