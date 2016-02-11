namespace MusicBeePlugin
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Timers;
    using System.Windows.Forms;

    using MusicBeePlugin.AndroidRemote.Entities;
    using MusicBeePlugin.AndroidRemote.Events;
    using MusicBeePlugin.ApiAdapters;

    using MusicBeeRemoteCore;
    using MusicBeeRemoteCore.Interfaces;

    using Timer = System.Timers.Timer;

    /// <summary>
    ///     The MusicBee Plugin class. Used to communicate with the MusicBee API.
    /// </summary>
    public partial class Plugin : IMessageHandler
    {
        /// <summary>
        ///     The about.
        /// </summary>
        private readonly PluginInfo about = new PluginInfo();

        /// <summary>
        ///     The mb api interface.
        /// </summary>
        private MusicBeeApiInterface api;

        private MusicBeeRemoteEntryPoint mbrc;

        private Timer positionUpdateTimer;

        private RepeatMode repeat;

        private bool scrobble;

        private bool shuffle;

        private string storagePath;

        private Timer timer;

        private InfoWindow window;

        /// <summary>
        ///     The close.
        /// </summary>
        /// <param name="reason">
        ///     The reason.
        /// </param>
        public void Close(PluginCloseReason reason)
        {
            this.mbrc.getBus().Publish(new MessageEvent(MessageEvent.ActionSocketStop));
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
            this.DisplayInfoWindow();
            return true;
        }

        /// <summary>
        ///     This function initialized the Plugin.
        /// </summary>
        /// <param name="apiInterfacePtr"></param>
        /// <returns></returns>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            this.InitializeAbout();

            this.api = new MusicBeeApiInterface();
            this.api.Initialise(apiInterfacePtr);
            var supportedApi = this.api.ApiRevision >= MinApiRevision;

            this.storagePath = this.api.Setting_GetPersistentStoragePath() + "\\mb_remote";

            this.mbrc = new MusicBeeRemoteEntryPointImpl { StoragePath = this.storagePath };
            this.mbrc.setMessageHandler(this);
            try
            {
                this.mbrc.init(
            supportedApi,
            new BindingProviderImpl(
                new PlayerApiAdapter(this.api),
                new PlaylistApiAdapter(this.api),
                new TrackApiAdapter(this.api),
                new LibraryApiAdapter(this.api),
                new NowPlayingApiAdapter(this.api)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.OnMessageAvailable("MBRC failed to initialize");
                return this.about;
            }

            if (supportedApi)
            {
                this.api.MB_AddMenuItem(
                    "mnuTools/MusicBee Remote", 
                    "Information Panel of the MusicBee Remote", 
                    this.MenuItemClicked);
                this.UpdateCachedCover();
                this.UpdateCachedLyrics();
                this.StartPlayerStatusMonitoring();
            }
            
            return this.about;
        }

        public void OnMessageAvailable(string message)
        {
            this.api.MB_SetBackgroundTaskMessage(message);
        }

        /// <summary>
        /// </summary>
        public void OpenInfoWindow()
        {
            var hwnd = this.api.MB_GetWindowHandle();
            var mb = (Form)Control.FromHandle(hwnd);
            mb.Invoke(new MethodInvoker(this.DisplayInfoWindow));
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
                    this.UpdateCachedCover();
                    this.UpdateCachedLyrics();
                    this.mbrc.notify(NotificationMessage.TrackChanged);
                    break;
                case NotificationType.VolumeLevelChanged:
                    this.mbrc.notify(NotificationMessage.VolumeChanged, true);
                    break;
                case NotificationType.VolumeMuteChanged:
                    this.mbrc.notify(NotificationMessage.MuteStatusChanged);
                    break;
                case NotificationType.PlayStateChanged:
                    this.mbrc.notify(NotificationMessage.PlayStatusChanged);
                    break;
                case NotificationType.NowPlayingLyricsReady:
                    if (this.api.ApiRevision >= 17)
                    {
                        this.UpdateCachedLyrics();
                    }

                    break;
                case NotificationType.NowPlayingArtworkReady:
                    if (this.api.ApiRevision >= 17)
                    {
                        this.UpdateCachedCover();
                    }

                    break;
                case NotificationType.NowPlayingListChanged:
                    this.mbrc.notify(NotificationMessage.NowPlayingListChanged);
                    break;
                case NotificationType.PlayerRepeatChanged:
                    this.mbrc.notify(NotificationMessage.RepeatStatusChanged);
                    break;
                case NotificationType.PlayerShuffleChanged:
                    this.mbrc.notify(NotificationMessage.ShuffleStatusChanged);
                    break;
                case NotificationType.PlayerScrobbleChanged:
                    this.mbrc.notify(NotificationMessage.ScrobbleStatusChanged);
                    break;
                case NotificationType.AutoDjStarted:
                    this.mbrc.notify(NotificationMessage.AutoDjStarted);
                    break;
                case NotificationType.AutoDjStopped:
                    this.mbrc.notify(NotificationMessage.AutoDjStopped);
                    break;
                case NotificationType.RatingChanged:
                    this.mbrc.notify(NotificationMessage.RatingChanged);
                    break;
            }
        }

        /// <summary>
        ///     Called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        ///     Used to save the temporary Plugin SettingsModel if the have Changed.
        /// </summary>
        public void SaveSettings()
        {
            // Unused (Settings are explicitly saved on button click)
        }

        /// <summary>
        ///     Cleans up any persisted files during the plugin uninstall.
        /// </summary>
        public void Uninstall()
        {
            var settingsFolder = this.api.Setting_GetPersistentStoragePath + "\\mb_remote";
            if (Directory.Exists(settingsFolder))
            {
                Directory.Delete(settingsFolder);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="status"></param>
        public void UpdateWindowStatus(bool status)
        {
            if (this.window != null && this.window.Visible)
            {
                this.window.UpdateSocketStatus(status);
            }
        }

        private void DisplayInfoWindow()
        {
            if (this.window == null || !this.window.Visible)
            {
                this.window = new InfoWindow(this.mbrc.Settings);
                var cachedTracks = this.mbrc.CachedTrackCount;
                var cachedCovers = this.mbrc.CachedCoverCount;
                this.window.UpdateCacheStatus(cachedCovers, cachedTracks);
            }

            this.window.Show();
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
            if (this.api.Player_GetShuffle() != this.shuffle)
            {
                this.mbrc.notify(NotificationMessage.ShuffleStatusChanged);
                this.shuffle = this.api.Player_GetShuffle();
            }

            if (this.api.Player_GetScrobbleEnabled() != this.scrobble)
            {
                this.mbrc.notify(NotificationMessage.ScrobbleStatusChanged);
                this.scrobble = this.api.Player_GetScrobbleEnabled();
            }

            if (this.api.Player_GetRepeat() == this.repeat)
            {
                return;
            }

            this.mbrc.notify(NotificationMessage.RepeatStatusChanged);
            this.repeat = this.api.Player_GetRepeat();
        }

        private void InitializeAbout()
        {
            this.about.PluginInfoVersion = PluginInfoVersion;
            this.about.Name = "MusicBee Remote: Plugin";
            this.about.Description = "Remote Control for server to be used with android application.";
            this.about.Author = "Konstantinos Paparas (aka Kelsos)";
            this.about.TargetApplication = "MusicBee Remote";

            var v = Assembly.GetExecutingAssembly().GetName().Version;
            this.mbrc.setVersion(v.ToString());

            // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            this.about.Type = PluginType.General;
            this.about.VersionMajor = Convert.ToInt16(v.Major);
            this.about.VersionMinor = Convert.ToInt16(v.Minor);
            this.about.Revision = Convert.ToInt16(v.Revision);
            this.about.MinInterfaceVersion = MinInterfaceVersion;
            this.about.MinApiRevision = MinApiRevision;
            this.about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;
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
            this.DisplayInfoWindow();
        }

        /// <summary>
        ///     Starts the player status monitoring.
        /// </summary>
        private void StartPlayerStatusMonitoring()
        {
            this.scrobble = this.api.Player_GetScrobbleEnabled();
            this.repeat = this.api.Player_GetRepeat();
            this.shuffle = this.api.Player_GetShuffle();
            this.timer = new Timer { Interval = 1000 };
            this.timer.Elapsed += this.HandleTimerElapsed;
            this.timer.Enabled = true;
        }

        private void UpdateCachedCover()
        {
            var cover = this.api.NowPlaying_GetDownloadedArtwork() ?? this.api.NowPlaying_GetArtwork();

            this.mbrc.CacheCover(cover);
        }

        private void UpdateCachedLyrics()
        {
            var lyrics = this.api.NowPlaying_GetLyrics() ?? this.api.NowPlaying_GetDownloadedLyrics();

            this.mbrc.CacheLyrics(lyrics);
        }
    }
}