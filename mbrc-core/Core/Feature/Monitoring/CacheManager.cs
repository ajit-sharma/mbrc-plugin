using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Feature.Playlists;
using NLog;

namespace MusicBeeRemote.Core.Feature.Monitoring
{
    public class CacheManager
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly LibraryModule _libraryModule;
        private readonly IPlaylistModule _playlistModule;

        public CacheManager(LibraryModule libraryModule, IPlaylistModule playlistModule)
        {
            _libraryModule = libraryModule;
            _playlistModule = playlistModule;
        }

        private void BuildCache()
        {
            var observable = Observable.Create<string>(
                o =>
                {
                    o.OnNext(@"MBRC: building library cache.");
                    _libraryModule.BuildCache();
                    o.OnNext(@"MBRC: Synchronizing playlists.");
                    _playlistModule.SyncPlaylistsWithCache();
                    o.OnNext(@"MBRC: Processing album covers.");
                    _libraryModule.BuildCoverCachePerAlbum();
                    o.OnNext(@"MBRC: Cache Ready.");

                    o.OnCompleted();
                    return () => { };
                });

            observable.SubscribeOn(ThreadPoolScheduler.Instance)
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(
                    s =>
                    {
                        //notify the plugin messageHandler?.OnMessageAvailable(s);
                        _logger.Debug(s);
                    },
                    ex =>
                    {
                        _logger.Debug(ex, "Library sync failed");
                    });
        }

    }
}