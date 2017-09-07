using System;
using System.Net;
using MusicBeeRemote.Core.Events.Notifications;
using MusicBeeRemote.Core.Feature.Monitoring;
using MusicBeeRemote.Core.Network;
using MusicBeeRemote.Core.Settings;
using MusicBeeRemote.Core.Windows;
using Nancy.Hosting.Self;
using NLog;
using TinyMessenger;
using WebSocketProxy;
using Logger = NLog.Logger;

namespace MusicBeeRemote.Core
{
    public class MusicBeeRemotePlugin : IMusicBeeRemotePlugin
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Bootstrapper _bootstrapper;
        private readonly PersistenceManager _persistenceManager;
        private readonly SocketServer _socketServer;
        private readonly ServiceDiscovery _serviceDiscovery;

        private readonly ITinyMessengerHub _hub;

        private NancyHost _nancyHost;
        private readonly IWindowManager _windowManager;
        private readonly CacheManager _cacheManager;

        public MusicBeeRemotePlugin(
            Bootstrapper bootstrapper,
            PersistenceManager persistenceManager,
            SocketServer socketServer,
            ServiceDiscovery serviceDiscovery,
            IWindowManager windowManager,
            CacheManager cacheManager,
            ITinyMessengerHub hub
        )
        {
            _bootstrapper = bootstrapper;
            _persistenceManager = persistenceManager;
            _socketServer = socketServer;
            _serviceDiscovery = serviceDiscovery;
            _windowManager = windowManager;
            _cacheManager = cacheManager;

            _hub = hub;
        }

        public void Start()
        {
            _serviceDiscovery.Start();
            _socketServer.Start();
            StartHttp();
            StartProxy();
            _cacheManager.Start();
        }

        private void StartHttp()
        {
            try
            {
                var configuration = new HostConfiguration
                {
                    RewriteLocalhost = true,
                    UrlReservations =
                    {
                        CreateAutomatically = true
                    }
                };

                var port = _persistenceManager.UserSettingsModel.HttpPort;
                var listeningUri = new Uri($"http://localhost:{port}/");

                _nancyHost = new NancyHost(
                    listeningUri,
                    _bootstrapper,
                    configuration
                );

                _nancyHost.Start();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
            }
        }


        private void StartProxy()
        {
            var settings = _persistenceManager.UserSettingsModel;

            var configuration = new TcpProxyConfiguration
            {
                HttpHost = new Host
                {
                    Port = (int) settings.HttpPort,
                    IpAddress = IPAddress.Loopback
                },
                PublicHost = new Host
                {
                    Port = (int) settings.ProxyPort,
                    IpAddress = IPAddress.Parse("0.0.0.0")
                },
                WebSocketHost = new Host
                {
                    Port = (int) settings.WebSocketPort,
                    IpAddress = IPAddress.Loopback
                }
            };

            var tcpProxy = new TcpProxyServer(configuration);
            tcpProxy.Start();
        }

        public void Stop()
        {
            _serviceDiscovery.Stop();
            _socketServer.Stop();
            _nancyHost.Stop();
        }

        public void DisplayInfoWindow()
        {
            _windowManager.DisplayInfoWindow();
        }

        public void NotifyTrackChanged()
        {
            _hub.Publish(new TrackChangedEvent());
        }

        public void NotifyVolumeLevelChanged()
        {
            _hub.Publish(new VolumeLevelChangedEvent());
        }

        public void NotifyVolumeMuteChanged()
        {
            _hub.Publish(new VolumeMuteChangedEvent());
        }

        public void NotifyPlayStateChanged()
        {
            _hub.Publish(new PlayStateChangedEvent());
        }

        public void NotifyLyricsReady()
        {
            _hub.Publish(new LyricsReadyEvent());
        }

        public void NotifyArtworkReady()
        {
            _hub.Publish(new ArtworkReadyEvent());
        }

        public void NotifyNowPlayingListChanged()
        {
            _hub.Publish(new NowPlayingListChangedEvent());
        }

        public void DisplayPartyModeWindow()
        {
            _windowManager.DisplayPartyModeWindow();
        }
    }
}