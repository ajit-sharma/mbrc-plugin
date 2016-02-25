
namespace MusicBeeRemoteCore.Interfaces
{
    using MusicBeeRemoteCore;
    using MusicBeeRemoteCore.ApiAdapters;

    public interface IBindingProvider
    {
        IPlayerApiAdapter PlayerApi { get; }

        IPlaylistApiAdapter PlaylistApi { get; }
    
        ITrackApiAdapter TrackApi { get; }

        ILibraryApiAdapter LibraryApi { get; }

        INowPlayingApiAdapter NowPlayingApi { get; }
    }
}
