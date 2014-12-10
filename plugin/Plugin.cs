#region

using MusicBeePlugin.AndroidRemote;
using MusicBeePlugin.AndroidRemote.Controller;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;
using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.AndroidRemote.Networking;
using MusicBeePlugin.AndroidRemote.Settings;
using MusicBeePlugin.Debugging;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Ninject;
using NLog;
using NLog.Config;
using NLog.Targets;
using ServiceStack.Text;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using MusicBeePlugin.AndroidRemote.Utilities;
using Timer = System.Timers.Timer;

#endregion

namespace MusicBeePlugin
{
    /// <summary>
    ///     The MusicBee Plugin class. Used to communicate with the MusicBee API.
    /// </summary>
    public partial class Plugin
    {
        /// <summary>
        ///     The mb api interface.
        /// </summary>
        private MusicBeeApiInterface _api;

        /// <summary>
        ///     The _about.
        /// </summary>
        private readonly PluginInfo _about = new PluginInfo();

        private Timer _positionUpdateTimer;

        /// <summary>
        ///     Returns the plugin instance (Singleton);
        /// </summary>
        public static Plugin Instance { get; private set; }

        private InfoWindow _mWindow;

#if DEBUG
        private DebugTool _dTool;
#endif
        private string _mStoragePath;

        private Timer _timer;
        private bool _scrobble;
        private RepeatMode _repeat;
        private bool _shuffle;

        private StandardKernel _kernel;
        private SettingsController _settings;


        /// <summary>
        ///     This function initialized the Plugin.
        /// </summary>
        /// <param name="apiInterfacePtr"></param>
        /// <returns></returns>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            Instance = this;
            JsConfig.ExcludeTypeInfo = true;

            _api = new MusicBeeApiInterface();
            _api.Initialise(apiInterfacePtr);

            _mStoragePath = _api.Setting_GetPersistentStoragePath() + "\\mb_remote";

            InitializeLoggingConfiguration();

            InjectionModule.Api = _api;
            InjectionModule.StoragePath = _mStoragePath;

            Utilities.StoragePath = _mStoragePath;

            _kernel = new StandardKernel(new InjectionModule());

            _settings = _kernel.Get<SettingsController>();
            _settings.LoadSettings();

            InitializeAbout();

            if (_api.ApiRevision < MinApiRevision)
            {
                return _about;
            }

            var controller = _kernel.Get<Controller>();
            controller.InjectKernel(_kernel);
            Configuration.Register(controller);
            EventBus.Controller = controller;

            var libraryModule = _kernel.Get<LibraryModule>();
            var playlistModule = _kernel.Get<PlaylistModule>();

            if (libraryModule.IsCacheEmpty())
            {
                Task.Factory.StartNew(() =>
                {
                    libraryModule.BuildCache();
                    playlistModule.StoreAvailablePlaylists();
                    libraryModule.BuildCoverCachePerAlbum();
                });
            }

            UpdateCachedCover();
            UpdateCachedLyrics();

            _api.MB_AddMenuItem("mnuTools/MusicBee Remote", "Information Panel of the MusicBee Remote",
                MenuItemClicked);


            EventBus.FireEvent(new MessageEvent(MessageEvent.ActionSocketStart));
            EventBus.FireEvent(new MessageEvent(MessageEvent.InitializeModel));
            EventBus.FireEvent(new MessageEvent(MessageEvent.StartServiceBroadcast));
            EventBus.FireEvent(new MessageEvent(MessageEvent.ShowFirstRunDialog));

            _positionUpdateTimer = new Timer(20000);
            _positionUpdateTimer.Elapsed += PositionUpdateTimerOnElapsed;
            _positionUpdateTimer.Enabled = true;


#if DEBUG
            _api.MB_AddMenuItem("mnuTools/MBRC Debug Tool", "DebugTool",
                DisplayDebugWindow);
#endif

            StartPlayerStatusMonitoring();

            var appHost = _kernel.Get<AppHost>();
            appHost.Container.Adapter = new NinjectIocAdapter(_kernel);

            appHost.Init();
            appHost.Start(String.Format("http://+:{0}/", _settings.Settings.HttpPort));
            //Tools.NetworkTools.CreateFirewallRuleForPort((int)_settings.Settings.HttpPort);


            return _about;
        }

        private void InitializeAbout()
        {
            _about.PluginInfoVersion = PluginInfoVersion;
            _about.Name = "MusicBee Remote: Plugin";
            _about.Description = "Remote Control for server to be used with android application.";
            _about.Author = "Konstantinos Paparas (aka Kelsos)";
            _about.TargetApplication = "MusicBee Remote";

            var v = Assembly.GetExecutingAssembly().GetName().Version;
            _settings.Settings.CurrentVersion = v.ToString();

            // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            _about.Type = PluginType.General;
            _about.VersionMajor = Convert.ToInt16(v.Major);
            _about.VersionMinor = Convert.ToInt16(v.Minor);
            _about.Revision = Convert.ToInt16(v.Revision);
            _about.MinInterfaceVersion = MinInterfaceVersion;
            _about.MinApiRevision = MinApiRevision;
            _about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;
        }

        /// <summary>
        ///     Initializes the logging configuration.
        /// </summary>
        private void InitializeLoggingConfiguration()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            var fileTarget = new FileTarget();
            var debugger = new DebuggerTarget();

            config.AddTarget("console", consoleTarget);
            config.AddTarget("file", fileTarget);
            config.AddTarget("debugger", debugger);

            consoleTarget.Layout = @"${date:format=HH\\:MM\\:ss} ${logger} ${message} ${exception}";
            fileTarget.FileName = string.Format("{0}\\error.log", _mStoragePath);
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}||${exception}";

            debugger.Layout = fileTarget.Layout;

            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            var rule3 = new LoggingRule("*", LogLevel.Debug, debugger);
            config.LoggingRules.Add(rule3);

            LogManager.Configuration = config;
        }

        /// <summary>
        ///     Starts the player status monitoring.
        /// </summary>
        private void StartPlayerStatusMonitoring()
        {
            _scrobble = _api.Player_GetScrobbleEnabled();
            _repeat = _api.Player_GetRepeat();
            _shuffle = _api.Player_GetShuffle();
            _timer = new Timer { Interval = 1000 };
            _timer.Elapsed += HandleTimerElapsed;
            _timer.Enabled = true;
        }

        /// <summary>
        ///     This function runs periodically every 1000 ms as the timer ticks and
        ///     checks for changes on the player status. If a change is detected on
        ///     one of the monitored variables the function will fire an event with
        ///     the new status.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        private void HandleTimerElapsed(object sender, ElapsedEventArgs args)
        {
            if (_api.Player_GetShuffle() != _shuffle)
            {
                SendNotificationMessage(NotificationMessage.ShuffleStatusChanged);
            }
            if (_api.Player_GetScrobbleEnabled() != _scrobble)
            {
                SendNotificationMessage(NotificationMessage.ScrobbleStatusChanged);
            }

            if (_api.Player_GetRepeat() == _repeat) return;
            SendNotificationMessage(NotificationMessage.RepeatStatusChanged);
        }

        private void PositionUpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_api.Player_GetPlayState() == PlayState.Playing)
            {
                //PlayerModule.RequestPlayPosition("status");
            }
        }

        /// <summary>
        ///     Menu Item click handler. It handles the Tools -> MusicBee Remote entry click and opens the respective info panel.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private void MenuItemClicked(object sender, EventArgs args)
        {
            DisplayInfoWindow();
        }

        public void UpdateWindowStatus(bool status)
        {
            if (_mWindow != null && _mWindow.Visible)
            {
                _mWindow.UpdateSocketStatus(status);
            }
        }

        public void OpenInfoWindow()
        {
            var hwnd = _api.MB_GetWindowHandle();
            var mb = (Form)Control.FromHandle(hwnd);
            mb.Invoke(new MethodInvoker(DisplayInfoWindow));
        }

        private void DisplayInfoWindow()
        {
            if (_mWindow == null || !_mWindow.Visible)
            {
                _mWindow = _kernel.Get<InfoWindow>();
                var libraryModule = _kernel.Get<LibraryModule>();
                var cachedTracks = libraryModule.GetCachedEntitiesCount<LibraryTrack>();
                var cachedCovers = libraryModule.GetCachedEntitiesCount<LibraryCover>();
                _mWindow.UpdateCacheStatus(cachedCovers, cachedTracks);
            }

            _mWindow.Show();
        }

#if DEBUG
        public void DisplayDebugWindow(object sender, EventArgs eventArgs)
        {
            if (_dTool == null || !_dTool.Visible)
            {
                _dTool = new DebugTool();
            }
            _dTool.Show();
        }
#endif

        /// <summary>
        ///     Creates the MusicBee plugin Configuration panel.
        /// </summary>
        /// <param name="panelHandle">
        ///     The panel handle.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Configure(IntPtr panelHandle)
        {
            DisplayInfoWindow();
            return true;
        }

        /// <summary>
        ///     The close.
        /// </summary>
        /// <param name="reason">
        ///     The reason.
        /// </param>
        public void Close(PluginCloseReason reason)
        {
            /** When the plugin closes for whatever reason the SocketServer must stop **/
            EventBus.FireEvent(new MessageEvent(MessageEvent.ActionSocketStop));
        }

        /// <summary>
        ///     Cleans up any persisted files during the plugin uninstall.
        /// </summary>
        public void Uninstall()
        {
            var settingsFolder = _api.Setting_GetPersistentStoragePath + "\\mb_remote";
            if (Directory.Exists(settingsFolder))
            {
                Directory.Delete(settingsFolder);
            }
        }

        /// <summary>
        ///     Called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        ///     Used to save the temporary Plugin SettingsModel if the have changed.
        /// </summary>
        public void SaveSettings()
        {
            //Unused (Settings are explicitly saved on button click)
        }

        /// <summary>
        ///     Receives event Notifications from MusicBee. It is only required if the about.ReceiveNotificationFlags =
        ///     PlayerEvents.
        /// </summary>
        /// <param name="sourceFileUrl"></param>
        /// <param name="type"></param>
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            /** Perfom an action depending on the notification type **/
            switch (type)
            {
                case NotificationType.TrackChanged:
                    UpdateCachedCover();
                    UpdateCachedLyrics();
                    SendNotificationMessage(NotificationMessage.TrackChanged);
                    break;
                case NotificationType.VolumeLevelChanged:
                    SendNotificationMessage(NotificationMessage.VolumeChanged);
                    break;
                case NotificationType.VolumeMuteChanged:
                    SendNotificationMessage(NotificationMessage.MuteStatusChanged);
                    break;
                case NotificationType.PlayStateChanged:
                    SendNotificationMessage(NotificationMessage.PlayStatusChanged);
                    break;
                case NotificationType.NowPlayingLyricsReady:
                    if (_api.ApiRevision >= 17)
                    {
                        UpdateCachedLyrics();
                    }
                    break;
                case NotificationType.NowPlayingArtworkReady:
                    if (_api.ApiRevision >= 17)
                    {
                        UpdateCachedCover();
                    }
                    break;
                case NotificationType.NowPlayingListChanged:
                    SendNotificationMessage(NotificationMessage.NowPlayingListChanged);
                    break;
                case NotificationType.PlayerRepeatChanged:
                    SendNotificationMessage(NotificationMessage.RepeatStatusChanged);
                    break;
                case NotificationType.PlayerShuffleChanged:
                    SendNotificationMessage(NotificationMessage.ShuffleStatusChanged);
                    break;
                case NotificationType.PlayerScrobbleChanged:
                    SendNotificationMessage(NotificationMessage.ScrobbleStatusChanged);
                    break;
                case NotificationType.AutoDjStarted:
                    SendNotificationMessage(NotificationMessage.AutoDjStarted);
                    break;
                case NotificationType.AutoDjStopped:
                    SendNotificationMessage(NotificationMessage.AutoDjStopped);
                    break;
            }
        }

        private void UpdateCachedLyrics()
        {
            var lyrics = _api.NowPlaying_GetLyrics()
                         ?? _api.NowPlaying_GetDownloadedLyrics();
            if (String.IsNullOrEmpty(lyrics))
            {
                lyrics = "Lyrics Not Found";
            }

            var model = _kernel.Get<LyricCoverModel>();
            model.Lyrics = lyrics;
        }

        private void UpdateCachedCover()
        {
            var model = _kernel.Get<LyricCoverModel>();
            var cover = _api.NowPlaying_GetDownloadedArtwork() ??
                        _api.NowPlaying_GetArtwork();

            model.SetCover(cover);
        }

        private void SendNotificationMessage(string message)
        {
            var server = _kernel.Get<SocketServer>();
            var notification = new NotificationMessage
            {
                Message = message
            };

            server.Send(notification.ToJsonString());
        }
    }
}