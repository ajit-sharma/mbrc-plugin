#region

using MusicBeePlugin.AndroidRemote;
using MusicBeePlugin.AndroidRemote.Controller;
using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;
using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.AndroidRemote.Networking;
using MusicBeePlugin.Modules;

using Ninject;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

using System.Timers;
using System.Windows.Forms;
using MusicBeePlugin.AndroidRemote.Persistence;

using Timer = System.Timers.Timer;

#endregion

namespace MusicBeePlugin
{
    using MusicBeeRemoteCore;

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

        private string _mStoragePath;

        private Timer _timer;
        private bool _scrobble;
        private RepeatMode _repeat;
        private bool _shuffle;

        private StandardKernel _kernel;
        private PersistenceController _persistence;
        private Subject<string> _volumeEventDebouncer =new Subject<string>();

    
        /// <summary>
        ///     This function initialized the Plugin.
        /// </summary>
        /// <param name="apiInterfacePtr"></param>
        /// <returns></returns>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            Instance = this;
			_api = new MusicBeeApiInterface();
            _api.Initialise(apiInterfacePtr);

            _mStoragePath = _api.Setting_GetPersistentStoragePath() + "\\mb_remote";

            var supportedApi = this._api.ApiRevision >= MinApiRevision;

            MusicBeeRemoteEntryPoint mbrc = new MusicBeeRemoteEntryPointImpl();
            mbrc.StoragePath = this._mStoragePath;
           
            mbrc.init(supportedApi);

            InitializeAbout();
            
            return _about;
            

            

            UpdateCachedCover();
            UpdateCachedLyrics();

            _api.MB_AddMenuItem("mnuTools/MusicBee Remote", "Information Panel of the MusicBee Remote", MenuItemClicked);

            EventBus.FireEvent(new MessageEvent(MessageEvent.ActionSocketStart));
            EventBus.FireEvent(new MessageEvent(MessageEvent.StartServiceBroadcast));
            EventBus.FireEvent(new MessageEvent(MessageEvent.ShowFirstRunDialog));

            StartPlayerStatusMonitoring();

            


            
      
            _volumeEventDebouncer.Throttle(TimeSpan.FromSeconds(1)).Subscribe(SendNotificationMessage);
           
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
            _persistence.Settings.CurrentVersion = v.ToString();

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
                _shuffle = _api.Player_GetShuffle();
            }
            if (_api.Player_GetScrobbleEnabled() != _scrobble)
            {
                SendNotificationMessage(NotificationMessage.ScrobbleStatusChanged);
                _scrobble = _api.Player_GetScrobbleEnabled();
            }

            if (_api.Player_GetRepeat() == _repeat) return;
            SendNotificationMessage(NotificationMessage.RepeatStatusChanged);
            _repeat = _api.Player_GetRepeat();
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

	    /// <summary>
	    /// </summary>
	    /// <param name="status"></param>
	    public void UpdateWindowStatus(bool status)
        {
            if (_mWindow != null && _mWindow.Visible)
            {
                _mWindow.UpdateSocketStatus(status);
            }
        }

	    /// <summary>
	    /// </summary>
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
                var cachedTracks = libraryModule.GetCachedTrackCount();
                var cachedCovers = libraryModule.GetCachedCoverCount();
                _mWindow.UpdateCacheStatus(cachedCovers, cachedTracks);
            }

            _mWindow.Show();
        }

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
        ///     Used to save the temporary Plugin SettingsModel if the have Changed.
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
                    _volumeEventDebouncer.OnNext(NotificationMessage.VolumeChanged);
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
                case NotificationType.RatingChanged:
                    SendNotificationMessage(NotificationMessage.RatingChanged);
                    break;
            }
        }

        private void UpdateCachedLyrics()
        {
            var lyrics = _api.NowPlaying_GetLyrics()
                         ?? _api.NowPlaying_GetDownloadedLyrics();
            if (string.IsNullOrEmpty(lyrics))
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