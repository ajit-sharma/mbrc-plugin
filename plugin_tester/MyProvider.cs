namespace plugin_tester
{
    using MusicBeePlugin;
    using MusicBeePlugin.ApiAdapters;

    using MusicBeeRemoteCore.Interfaces;

    internal class MyProvider : IBindingProvider
    {
        public MyProvider(IPlayerApiAdapter playerApi,
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

        public IPlayerApiAdapter PlayerApi { get; }

        public IPlaylistApiAdapter PlaylistApi { get; }

        public ITrackApiAdapter TrackApi { get; }

        public ILibraryApiAdapter LibraryApi { get; }

        public INowPlayingApiAdapter NowPlayingApi { get; }
    }
}