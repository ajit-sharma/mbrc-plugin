namespace MusicBeePlugin
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Timers;
    using System.Windows.Forms;
    using MusicBeePlugin.ApiAdapters;
    using MusicBeeRemoteCore;
    using MusicBeeRemoteCore.AndroidRemote.Entities;
    using MusicBeeRemoteCore.AndroidRemote.Events;
    using MusicBeeRemoteCore.Interfaces;
    using Timer = System.Timers.Timer;

    /// <summary>
    ///     The MusicBee Plugin class. Used to communicate with the MusicBee API.
    /// </summary>
    public partial class Plugin : IMessageHandler
    {
        /// <summary>
        ///     The mb api interface.
        /// </summary>
        private MusicBeeApiInterface api;

        private IMusicBeeRemoteEntryPoint mbrc;

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
            this.mbrc.GetBus().Publish(new MessageEvent(MessageEvent.ActionSocketStop));
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
        ///     This function initialized the plugin
        /// </summary>
        /// <param name="apiInterfacePtr">
        /// The MusicBee API interface pointer
        /// </param>
        /// <returns>
        /// The basic information of the plugin.
        /// </returns>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            this.api = new MusicBeeApiInterface();
            this.api.Initialise(apiInterfacePtr);
            var supportedApi = this.api.ApiRevision >= MinApiRevision;

            this.storagePath = this.api.Setting_GetPersistentStoragePath() + "\\mb_remote";

            this.mbrc = new MusicBeeRemoteEntryPointImpl {StoragePath = this.storagePath};

            this.mbrc.SetMessageHandler(this);

            try
            {
                this.mbrc.Init(
                    new BindingProviderImpl(
                        new PlayerApiAdapter(this.api),
                        new PlaylistApiAdapter(this.api),
                        new TrackApiAdapter(this.api),
                        new LibraryApiAdapter(this.api),
                        new NowPlayingApiAdapter(this.api)));

                this.mbrc.SetVersion(GetVersion().ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.OnMessageAvailable("MBRC failed to initialize");
                return InitializePluginInfo();
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

            return InitializePluginInfo();
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
            var mb = (Form) Control.FromHandle(hwnd);
            mb.Invoke(new MethodInvoker(this.DisplayInfoWindow));
        }

        /// <summary>
        ///     Receives event Notifications from MusicBee. It is only required if the pluginInfo.ReceiveNotificationFlags =
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
                    this.mbrc.Notify(NotificationMessage.TrackChanged);
                    break;
                case NotificationType.VolumeLevelChanged:
                    this.mbrc.Notify(NotificationMessage.VolumeChanged, true);
                    break;
                case NotificationType.VolumeMuteChanged:
                    this.mbrc.Notify(NotificationMessage.MuteStatusChanged);
                    break;
                case NotificationType.PlayStateChanged:
                    this.mbrc.Notify(NotificationMessage.PlayStatusChanged);
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
                    this.mbrc.Notify(NotificationMessage.NowPlayingListChanged);
                    break;
                case NotificationType.PlayerRepeatChanged:
                    this.mbrc.Notify(NotificationMessage.RepeatStatusChanged);
                    break;
                case NotificationType.PlayerShuffleChanged:
                    this.mbrc.Notify(NotificationMessage.ShuffleStatusChanged);
                    break;
                case NotificationType.PlayerScrobbleChanged:
                    this.mbrc.Notify(NotificationMessage.ScrobbleStatusChanged);
                    break;
                case NotificationType.AutoDjStarted:
                    this.mbrc.Notify(NotificationMessage.AutoDjStarted);
                    break;
                case NotificationType.AutoDjStopped:
                    this.mbrc.Notify(NotificationMessage.AutoDjStopped);
                    break;
                case NotificationType.RatingChanged:
                    this.mbrc.Notify(NotificationMessage.RatingChanged);
                    break;
                case NotificationType.FileAddedToLibrary:
                    this.mbrc.FileAdded(sourceFileUrl);
                    break;
                case NotificationType.FileDeleted:
                    this.mbrc.FileDeleted(sourceFileUrl);
                    break;
                case NotificationType.TagsChanged:
                    this.mbrc.TagsChanged(sourceFileUrl);
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
        /// Updates the socket status on the information window, if the window is visible
        /// </summary>
        /// <param name="status">
        /// True if the socket is listening for incoming connections and false if not.
        /// </param>
        public void UpdateWindowStatus(bool status)
        {
            if (this.window != null && this.window.Visible)
            {
                this.window.UpdateSocketStatus(status);
            }
        }

        /// <summary>
        /// Gets the plugin's version as provided by the Assembly
        /// </summary>
        /// <returns>
        /// The <see cref="Version"/>.
        /// </returns>
        private static Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Initializes the pluginInfo information of the plugin.
        /// </summary>
        /// <returns>
        /// The <see cref="PluginInfo"/>.
        /// </returns>
        private static PluginInfo InitializePluginInfo()
        {
            var version = GetVersion();
            const string Description = "Remote Control for server to be used with android application.";

            var pluginInfo = new PluginInfo
            {
                PluginInfoVersion = PluginInfoVersion,
                Name = "MusicBee Remote: Plugin",
                Description = Description,
                Author = "Konstantinos Paparas (aka Kelsos)",
                TargetApplication = "MusicBee Remote",
                Type = PluginType.General,
                VersionMajor = Convert.ToInt16(version.Major),
                VersionMinor = Convert.ToInt16(version.Minor),
                Revision = Convert.ToInt16(version.Revision),
                MinInterfaceVersion = MinInterfaceVersion,
                MinApiRevision = MinApiRevision,
                ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents
            };
            return pluginInfo;
        }

        /// <summary>
        /// Creates and shows the plugin configuration and information window.
        /// </summary>
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
        ///     This function runs periodically every 1000 milliseconds as the timer ticks and
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
                this.mbrc.Notify(NotificationMessage.ShuffleStatusChanged);
                this.shuffle = this.api.Player_GetShuffle();
            }

            if (this.api.Player_GetScrobbleEnabled() != this.scrobble)
            {
                this.mbrc.Notify(NotificationMessage.ScrobbleStatusChanged);
                this.scrobble = this.api.Player_GetScrobbleEnabled();
            }

            if (this.api.Player_GetRepeat() == this.repeat)
            {
                return;
            }

            this.mbrc.Notify(NotificationMessage.RepeatStatusChanged);
            this.repeat = this.api.Player_GetRepeat();
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
            this.timer = new Timer {Interval = 1000};
            this.timer.Elapsed += this.HandleTimerElapsed;
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Updates the cache with the current track's cover.
        /// </summary>
        private void UpdateCachedCover()
        {
            var cover = this.api.NowPlaying_GetDownloadedArtwork() ?? this.api.NowPlaying_GetArtwork();

            this.mbrc.CacheCover(cover);
        }

        /// <summary>
        /// Updates the cache with the current track's lyrics.
        /// </summary>
        private void UpdateCachedLyrics()
        {
            var lyrics = this.api.NowPlaying_GetLyrics() ?? this.api.NowPlaying_GetDownloadedLyrics();

            this.mbrc.CacheLyrics(lyrics);
        }
    }
}