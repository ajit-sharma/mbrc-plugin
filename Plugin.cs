using MusicBeePlugin.Rest;

namespace MusicBeePlugin
{
    using AndroidRemote.Data;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using System.Linq;
    using Debugging;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using AndroidRemote;
    using AndroidRemote.Events;
    using AndroidRemote.Networking;
    using ServiceStack.Text;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Timers;
    using AndroidRemote.Controller;
    using AndroidRemote.Entities;
    using AndroidRemote.Settings;
    using AndroidRemote.Enumerations;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// The MusicBee Plugin class. Used to communicate with the MusicBee API.
    /// </summary>
    public partial class Plugin
    {
        /// <summary>
        /// The mb api interface.
        /// </summary>
        private MusicBeeApiInterface _api;

        /// <summary>
        /// The _about.
        /// </summary>
        private readonly PluginInfo _about = new PluginInfo();

        private Timer _positionUpdateTimer;

        /// <summary>
        /// Returns the plugin instance (Singleton);
        /// </summary>
        public static Plugin Instance { get; private set; }

        private InfoWindow _mWindow;

#if DEBUG
        private DebugTool _dTool;
#endif
        private string _mStoragePath;

        public LibraryModule LibraryModule { get; private set; }

        public PlaylistModule PlaylistModule { get; private set; }
        public  NowPlayingModule NowPlayingModule { get; private set; }

        public PlayerModule PlayerModule { get; private set; }

        private Timer _timer;
        private bool _scrobble;
        private RepeatMode _repeat;
        private bool _shuffle;

        private CacheHelper _mHelper;


        /// <summary>
        /// This function initialized the Plugin.
        /// </summary>
        /// <param name="apiInterfacePtr"></param>
        /// <returns></returns>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            Instance = this;
            JsConfig.ExcludeTypeInfo = true;
            Configuration.Register(Controller.Instance);

            _api = new MusicBeeApiInterface();
            _api.Initialise(apiInterfacePtr);

            UserSettings.Instance.SetStoragePath(_api.Setting_GetPersistentStoragePath());
            UserSettings.Instance.LoadSettings();

            _about.PluginInfoVersion = PluginInfoVersion;
            _about.Name = "MusicBee Remote: Plugin";
            _about.Description = "Remote Control for server to be used with android application.";
            _about.Author = "Konstantinos Paparas (aka Kelsos)";
            _about.TargetApplication = "MusicBee Remote";

            var v = Assembly.GetExecutingAssembly().GetName().Version;
            UserSettings.Instance.CurrentVersion = v.ToString();

            // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            _about.Type = PluginType.General;
            _about.VersionMajor = Convert.ToInt16(v.Major);
            _about.VersionMinor = Convert.ToInt16(v.Minor);
            _about.Revision = Convert.ToInt16(v.Revision);
            _about.MinInterfaceVersion = MinInterfaceVersion;
            _about.MinApiRevision = MinApiRevision;
            _about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;

            if (_api.ApiRevision < MinApiRevision)
            {
                return _about;
            }

            _mStoragePath = _api.Setting_GetPersistentStoragePath() + "\\mb_remote";

            InitializeLoggingConfiguration();

            _api.MB_AddMenuItem("mnuTools/MusicBee Remote", "Information Panel of the MusicBee Remote",
                                          MenuItemClicked);

            EventBus.FireEvent(new MessageEvent(EventType.ActionSocketStart));
            EventBus.FireEvent(new MessageEvent(EventType.InitializeModel));
            EventBus.FireEvent(new MessageEvent(EventType.StartServiceBroadcast));
            EventBus.FireEvent(new MessageEvent(EventType.ShowFirstRunDialog));

            _positionUpdateTimer = new Timer(20000);
            _positionUpdateTimer.Elapsed += PositionUpdateTimerOnElapsed;
            _positionUpdateTimer.Enabled = true;

            InjectionModule.Api = _api;
            InjectionModule.StoragePath = _mStoragePath;
            
            LibraryModule = new LibraryModule(_api, _mStoragePath);
            PlaylistModule = new PlaylistModule(_api, _mStoragePath);
            NowPlayingModule = new NowPlayingModule(_api, _mStoragePath);
            PlayerModule = new PlayerModule(_api);

#if DEBUG
            _api.MB_AddMenuItem("mnuTools/MBRC Debug Tool", "DebugTool",
                                          DisplayDebugWindow);
#endif

            LibraryModule.CheckCacheState();
            PlaylistModule.StoreAvailablePlaylists();
            StartPlayerStatusMonitoring();
            _mHelper = new CacheHelper(_mStoragePath);

            var appHost = new AppHost();
            appHost.Init();
            appHost.Start("http://+:8188/");

            return _about;
        }

        /// <summary>
        /// Initializes the logging configuration.
        /// </summary>
        private void InitializeLoggingConfiguration()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            var fileTarget = new FileTarget();
            config.AddTarget("console", consoleTarget);
            config.AddTarget("file", fileTarget);

            consoleTarget.Layout = @"${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            fileTarget.FileName = string.Format("{0}\\error.log", _mStoragePath);
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";

            var rule1 = new LoggingRule("*", LogLevel.Info, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            LogManager.Configuration = config;
        }

        /// <summary>
        /// Starts the player status monitoring.
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
        /// This function runs periodically every 1000 ms as the timer ticks and
        /// checks for changes on the player status. If a change is detected on
        /// one of the monitored variables the function will fire an event with
        /// the new status.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        private void HandleTimerElapsed(object sender, ElapsedEventArgs args)
        {
            if (_api.Player_GetShuffle() != _shuffle)
            {
                _shuffle = _api.Player_GetShuffle();
                //SendSocketMessage(Constants.PlayerShuffle, Constants.Message, _shuffle);
            }
            if (_api.Player_GetScrobbleEnabled() != _scrobble)
            {
                _scrobble = _api.Player_GetScrobbleEnabled();
                //SendSocketMessage(Constants.PlayerScrobble, Constants.Message, _scrobble);
            }

            if (_api.Player_GetRepeat() == _repeat) return;
            _repeat = _api.Player_GetRepeat();
            //SendSocketMessage(Constants.PlayerRepeat, Constants.Message, _repeat);
        }

        private void PositionUpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_api.Player_GetPlayState() == PlayState.Playing)
            {
                //PlayerModule.RequestPlayPosition("status");
            }
        }

        /// <summary>
        /// Menu Item click handler. It handles the Tools -> MusicBee Remote entry click and opens the respective info panel.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
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
                _mWindow = new InfoWindow();    
            }
            
            _mWindow.Show();
            if (_mHelper != null)
            {
                _mWindow.UpdateCacheStatus(6, 5);
            }
            else
            {
                _mWindow.UpdateCacheStatus(0, 0);
            }
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
        /// Creates the MusicBee plugin Configuration panel.
        /// </summary>
        /// <param name="panelHandle">
        /// The panel handle.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Configure(IntPtr panelHandle)
        {
            DisplayInfoWindow();
            return true;
        }

        /// <summary>
        /// The close.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void Close(PluginCloseReason reason)
        {
            /** When the plugin closes for whatever reason the SocketServer must stop **/
            EventBus.FireEvent(new MessageEvent(EventType.ActionSocketStop));
        }

        /// <summary>
        /// Cleans up any persisted files during the plugin uninstall.
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
        /// Called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        /// Used to save the temporary Plugin SettingsModel if the have changed.
        /// </summary>
        public void SaveSettings()
        {
            //Unused (Settings are explicitly saved on button click)
        }

        /// <summary>
        /// Receives event Notifications from MusicBee. It is only required if the about.ReceiveNotificationFlags = PlayerEvents.
        /// </summary>
        /// <param name="sourceFileUrl"></param>
        /// <param name="type"></param>
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            /** Perfom an action depending on the notification type **/
            switch (type)
            {
                case NotificationType.TrackChanged:
//                    PlayerModule.RequestNowPlayingTrackCover();
//                    PlayerModule.RequestTrackRating(String.Empty, String.Empty);
//                    PlayerModule.RequestLoveStatus("status");
//                    PlayerModule.RequestNowPlayingTrackLyrics();
//                    PlayerModule.RequestPlayPosition("status");
                    //SendSocketMessage(Constants.NowPlayingTrack, Constants.Message, MusicBeePlugin.PlayerModule.GetTrackInfo());
                    break;
                case NotificationType.VolumeLevelChanged:
                    //SendSocketMessage(Constants.PlayerVolume, Constants.Message,((int) Math.Round(_api.Player_GetVolume()*100,1)));
                    break;
                case NotificationType.VolumeMuteChanged:
                    //SendSocketMessage(Constants.PlayerMute, Constants.Message, _api.Player_GetMute());
                    break;
                case NotificationType.PlayStateChanged:
                    //SendSocketMessage(Constants.PlayerState, Constants.Message,_api.Player_GetPlayState());
                    break;
                case NotificationType.NowPlayingLyricsReady:
                    if (_api.ApiRevision >= 17)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.NowPlayingLyricsChange,
                            !String.IsNullOrEmpty(_api.NowPlaying_GetDownloadedLyrics())
                                ? _api.NowPlaying_GetDownloadedLyrics() : "Lyrics Not Found" ));
                    }
                    break;
                case NotificationType.NowPlayingArtworkReady:
                    if (_api.ApiRevision >= 17)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange,
                                                            _api.NowPlaying_GetDownloadedArtwork()));
                    }
                    break;
                case NotificationType.NowPlayingListChanged:
                    //SendSocketMessage(Constants.NowPlayingListChanged, Constants.Message, true);
                    break;
                case NotificationType.PlayerRepeatChanged :
                    var repeat = _api.Player_GetRepeat();
                    //SendSocketMessage(Constants.PlayerRepeat, Constants.Message, repeat);
                    break;
                case NotificationType.PlayerShuffleChanged:
                    var shuffle = _api.Player_GetShuffle();
                    //SendSocketMessage(Constants.PlayerShuffle, Constants.Message, shuffle);
                    break;
                case NotificationType.PlayerScrobbleChanged:
                    var scrobble = _api.Player_GetScrobbleEnabled();
                    //SendSocketMessage(Constants.PlayerScrobble, Constants.Message, scrobble);
                    break;
                case NotificationType.AutoDjStarted:
                    //SendSocketMessage(Constants.PlayerAutoDj, Constants.Reply, true);
                    break;
                case NotificationType.AutoDjStopped:
                    //SendSocketMessage(Constants.PlayerAutoDj, Constants.Reply, false);
                    break;
            }
        }

        private static string XmlFilter(IEnumerable<string> tags, string query, bool isStrict)
        {
            var filter = new XElement("Source", new XAttribute("Type", 1));
            var conditions = new XElement("Conditions", new XAttribute("CombineMethod", "Any"));
            foreach (var condition in tags.Select(tag => new XElement("Condition",
                new XAttribute("Field", tag),
                new XAttribute("Comparison", isStrict ? "Is" : "Contains"),
                new XAttribute("Value", query))))
            {
                conditions.Add(condition);
            }
            filter.Add(conditions);

            return filter.ToString();
        }
        
        public string[] GetUrlsForTag(MetaTag tag, string query)
        {
            var filter = String.Empty;
            string[] tracks = {};
            if (tag != MetaTag.track)
            {
                switch (tag)
                {
                    case MetaTag.artist:
                        filter = XmlFilter(new[] {"ArtistPeople"}, query, true);
                        break;
                    case MetaTag.album:
                        filter = XmlFilter(new[] {"Album"}, query, true);
                        break;
                    case MetaTag.genre:
                        filter = XmlFilter(new[] {"Genre"}, query, true);
                        break;
                }

                _api.Library_QueryFilesEx(filter, ref tracks);

                var list = tracks.Select(file => new MetaData
                {
                    file = file,
                    artist = _api.Library_GetFileTag(file, MetaDataType.Artist),
                    album_artist = _api.Library_GetFileTag(file, MetaDataType.AlbumArtist),
                    album = _api.Library_GetFileTag(file, MetaDataType.Album),
                    title = _api.Library_GetFileTag(file, MetaDataType.TrackTitle),
                    genre = _api.Library_GetFileTag(file, MetaDataType.Genre),
                    year = _api.Library_GetFileTag(file, MetaDataType.Year),
                    track_no = _api.Library_GetFileTag(file, MetaDataType.TrackNo),
                    disc = _api.Library_GetFileTag(file, MetaDataType.DiscNo)
                }).ToList();
                list.Sort();
                tracks = list.Select(r => r.file)
                    .ToArray();
            }
            else
            {
                tracks = new[] {query};
            }
                
            return tracks;
        }
    }
}
