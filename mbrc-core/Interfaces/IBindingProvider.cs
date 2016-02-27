namespace MusicBeeRemoteCore.Interfaces
{
    using MusicBeeRemoteCore.ApiAdapters;

    public interface IBindingProvider
    {
        ILibraryApiAdapter LibraryApi { get; }

        INowPlayingApiAdapter NowPlayingApi { get; }

        IPlayerApiAdapter PlayerApi { get; }

        IPlaylistApiAdapter PlaylistApi { get; }

        ITrackApiAdapter TrackApi { get; }
    }
}