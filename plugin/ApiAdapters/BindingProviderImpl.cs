namespace MusicBeeRemoteCore.ApiAdapters
{
    using MusicBeeRemoteCore.Interfaces;

    class BindingProviderImpl : IBindingProvider
    {
        public BindingProviderImpl(
            IPlayerApiAdapter playerApi, 
            IPlaylistApiAdapter playlistApi, 
            ITrackApiAdapter trackApi, 
            ILibraryApiAdapter libraryApi, 
            INowPlayingApiAdapter nowPlayingApi)
        {
            this.PlayerApi = playerApi;
            this.PlaylistApi = playlistApi;
            this.TrackApi = trackApi;
            this.LibraryApi = libraryApi;
            this.NowPlayingApi = nowPlayingApi;
        }

        public ILibraryApiAdapter LibraryApi { get; }

        public INowPlayingApiAdapter NowPlayingApi { get; }

        public IPlayerApiAdapter PlayerApi { get; }

        public IPlaylistApiAdapter PlaylistApi { get; }

        public ITrackApiAdapter TrackApi { get; }
    }
}