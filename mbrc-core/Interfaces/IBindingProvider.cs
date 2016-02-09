
namespace MusicBeeRemoteCore.Interfaces
{
    using MusicBeePlugin;
    using MusicBeePlugin.ApiAdapters;

    public interface IBindingProvider
    {
        IPlayerApiAdapter PlayerApi { get; }

        IPlaylistApiAdapter PlaylistApi { get; }
    
        ITrackApiAdapter TrackApi { get; }

        ILibraryApiAdapter LibraryApi { get; }

        INowPlayingApiAdapter NowPlayingApi { get; }
    }
}
