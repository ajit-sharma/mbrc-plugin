using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MusicBeePlugin.AndroidRemote.Data;

namespace MusicBeePlugin
{
    using Debugging;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using AndroidRemote;
    using AndroidRemote.Events;
    using AndroidRemote.Networking;
    using ServiceStack.Text;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Timers;
    using AndroidRemote.Controller;
    using AndroidRemote.Entities;
    using AndroidRemote.Error;
    using AndroidRemote.Settings;
    using AndroidRemote.Utilities;
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
        private MusicBeeApiInterface api;

        /// <summary>
        /// The _about.
        /// </summary>
        private readonly PluginInfo about = new PluginInfo();

        /// <summary>
        /// The timer.
        /// </summary>
        private Timer timer;

        private Timer positionUpdateTimer;

        /// <summary>
        /// The shuffle.
        /// </summary>
        private bool shuffle;

        /// <summary>
        /// Represents the current repeat mode.
        /// </summary>
        private RepeatMode repeat;

        /// <summary>
        /// The scrobble.
        /// </summary>
        private bool scrobble;

        /// <summary>
        /// Returns the plugin instance (Singleton);
        /// </summary>
        public static Plugin Instance
        {
            get { return selfInstance; }
        }

        private static Plugin selfInstance;
        private InfoWindow mWindow;
        private CacheHelper mHelper;

#if DEBUG
        private DebugTool dTool;
#endif
        private string mStoragePath;

        /// <summary>
        /// This function initialized the Plugin.
        /// </summary>
        /// <param name="apiInterfacePtr"></param>
        /// <returns></returns>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            selfInstance = this;
            JsConfig.ExcludeTypeInfo = true;
            Configuration.Register(Controller.Instance);

            api = new MusicBeeApiInterface();
            api.Initialise(apiInterfacePtr);

            UserSettings.Instance.SetStoragePath(api.Setting_GetPersistentStoragePath());
            UserSettings.Instance.LoadSettings();

            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "MusicBee Remote: Plugin";
            about.Description = "Remote Control for server to be used with android application.";
            about.Author = "Konstantinos Paparas (aka Kelsos)";
            about.TargetApplication = "MusicBee Remote";

            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            UserSettings.Instance.CurrentVersion = v.ToString();

            // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = Convert.ToInt16(v.Major);
            about.VersionMinor = Convert.ToInt16(v.Minor);
            about.Revision = Convert.ToInt16(v.Revision);
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;

            if (api.ApiRevision < MinApiRevision)
            {
                return about;
            }

            ErrorHandler.SetLogFilePath(api.Setting_GetPersistentStoragePath());
            mStoragePath = api.Setting_GetPersistentStoragePath() + "\\mb_remote";

            StartPlayerStatusMonitoring();

            api.MB_AddMenuItem("mnuTools/MusicBee Remote", "Information Panel of the MusicBee Remote",
                                          MenuItemClicked);

            EventBus.FireEvent(new MessageEvent(EventType.ActionSocketStart));
            EventBus.FireEvent(new MessageEvent(EventType.InitializeModel));
            EventBus.FireEvent(new MessageEvent(EventType.StartServiceBroadcast));
            EventBus.FireEvent(new MessageEvent(EventType.ShowFirstRunDialog));

            positionUpdateTimer = new Timer(20000);
            positionUpdateTimer.Elapsed += PositionUpdateTimerOnElapsed;
            positionUpdateTimer.Enabled = true;

            mHelper = new CacheHelper(mStoragePath);

#if DEBUG
            api.MB_AddMenuItem("mnuTools/MBRC Debug Tool", "DebugTool",
                                          DisplayDebugWindow);
#endif

            return about;
        }

        private void PositionUpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (api.Player_GetPlayState() == PlayState.Playing)
            {
                RequestPlayPosition("status");
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
            if (mWindow != null && mWindow.Visible)
            {
                mWindow.UpdateSocketStatus(status);
            }
        }

        /// <summary>
        /// The function populates the local player status variables and then
        /// starts the Monitoring of the player status every 1000 ms to retrieve
        /// any changes.
        /// </summary>
        private void StartPlayerStatusMonitoring()
        {
            scrobble = api.Player_GetScrobbleEnabled();
            repeat = api.Player_GetRepeat();
            shuffle = api.Player_GetShuffle();
            timer = new Timer {Interval = 1000};
            timer.Elapsed += HandleTimerElapsed;
            timer.Enabled = true;
        }

        /// <summary>
        /// This function runs periodically every 1000 ms as the timer ticks and
        /// checks for changes on the player status.  If a change is detected on
        /// one of the monitored variables the function will fire an event with
        /// the new status.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The event arguments.
        /// </param>
        private void HandleTimerElapsed(object sender, ElapsedEventArgs args)
        {
            if (api.Player_GetShuffle() != shuffle)
            {
                shuffle = api.Player_GetShuffle();
                SendSocketMessage(Constants.PlayerShuffle, Constants.Message, shuffle);
            }
            if (api.Player_GetScrobbleEnabled() != scrobble)
            {
                scrobble = api.Player_GetScrobbleEnabled();
                SendSocketMessage(Constants.PlayerScrobble, Constants.Message, scrobble);
            }

            if (api.Player_GetRepeat() == repeat) return;
            repeat = api.Player_GetRepeat();
            SendSocketMessage(Constants.PlayerRepeat, Constants.Message, repeat);
        }

        public void OpenInfoWindow()
        {
            IntPtr hwnd = api.MB_GetWindowHandle();
            Form MB = (Form)Control.FromHandle(hwnd);
            MB.Invoke(new MethodInvoker(DisplayInfoWindow));
        }

        private void DisplayInfoWindow()
        {
            if (mWindow == null || !mWindow.Visible)
            {
                mWindow = new InfoWindow();    
            }

            mWindow.Show();    
        } 

#if DEBUG
        public void DisplayDebugWindow(object sender, EventArgs eventArgs)
        {
            if (dTool == null || !dTool.Visible)
            {
                dTool = new DebugTool();    
            }
            dTool.Show();
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
            string settingsFolder = api.Setting_GetPersistentStoragePath + "\\mb_remote";
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
            //UserSettings.SettingsModel = SettingsController.SettingsModel;
            //UserSettings.SaveSettings("mbremote");
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
                    RequestNowPlayingTrackCover();
                    RequestTrackRating(String.Empty, String.Empty);
                    RequestLoveStatus("status");
                    RequestNowPlayingTrackLyrics();
                    RequestPlayPosition("status");
                    SendSocketMessage(Constants.NowPlayingTrack, Constants.Message, GetTrackInfo());
                    break;
                case NotificationType.VolumeLevelChanged:
                    SendSocketMessage(Constants.PlayerVolume, Constants.Message,((int) Math.Round(api.Player_GetVolume()*100,1)));
                    break;
                case NotificationType.VolumeMuteChanged:
                    SendSocketMessage(Constants.PlayerMute, Constants.Message, api.Player_GetMute());
                    break;
                case NotificationType.PlayStateChanged:
                    SendSocketMessage(Constants.PlayerState, Constants.Message,api.Player_GetPlayState());
                    break;
                case NotificationType.NowPlayingLyricsReady:
                    if (api.ApiRevision >= 17)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.NowPlayingLyricsChange,
                            !String.IsNullOrEmpty(api.NowPlaying_GetDownloadedLyrics())
                                ? api.NowPlaying_GetDownloadedLyrics() : "Lyrics Not Found" ));
                    }
                    break;
                case NotificationType.NowPlayingArtworkReady:
                    if (api.ApiRevision >= 17)
                    {
                        EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange,
                                                            api.NowPlaying_GetDownloadedArtwork(), "",
                                                            api.NowPlaying_GetFileTag(MetaDataType.Album)));
                    }
                    break;
                case NotificationType.NowPlayingListChanged:
                    SendSocketMessage(Constants.NowPlayingListChanged, Constants.Message, true);
                    break;
            }
        }

        private NowPlayingTrack GetTrackInfo()
        {
            NowPlayingTrack nowPlayingTrack = new NowPlayingTrack();
            nowPlayingTrack.Artist = api.NowPlaying_GetFileTag(MetaDataType.Artist);
            nowPlayingTrack.Album = api.NowPlaying_GetFileTag(MetaDataType.Album);
            nowPlayingTrack.Year = api.NowPlaying_GetFileTag(MetaDataType.Year);
            nowPlayingTrack.SetTitle(api.NowPlaying_GetFileTag(MetaDataType.TrackTitle),
                                     api.NowPlaying_GetFileUrl());
            return nowPlayingTrack;
        }

        /// <summary>
        /// When called plays the next index.
        /// </summary>
        /// <returns></returns>
        public void RequestNextTrack(string clientId)
        {
            SendSocketMessage(Constants.PlayerNext, Constants.Reply, api.Player_PlayNextTrack());
        }

        /// <summary>
        /// When called stops the playback.
        /// </summary>
        /// <returns></returns>
        public void RequestStopPlayback(string clientId)
        {
            SendSocketMessage(Constants.PlayerStop, Constants.Reply, api.Player_Stop());
        }

        /// <summary>
        /// When called changes the play/pause state or starts playing a index if the status is stopped.
        /// </summary>
        /// <returns></returns>
        public void RequestPlayPauseTrack(string clientId)
        {
            SendSocketMessage(Constants.PlayerPlayPause, Constants.Reply, api.Player_PlayPause());
        }

        /// <summary>
        /// When called plays the previous index.
        /// </summary>
        /// <returns></returns>
        public void RequestPreviousTrack(string clientId)
        {
            SendSocketMessage(Constants.PlayerPrevious, Constants.Reply, api.Player_PlayPreviousTrack());
        }

        /// <summary>
        /// When called if the volume string is an integer in the range [0,100] it 
        /// changes the volume to the specific value and returns the new value.
        /// In any other case it just returns the current value for the volume.
        /// </summary>
        /// <param name="volume"> </param>
        public void RequestVolumeChange(int volume)
        {
            if (volume >= 0)
            {
                api.Player_SetVolume((float) volume/100);
            }

            SendSocketMessage(Constants.PlayerVolume, Constants.Reply, ((int)Math.Round(api.Player_GetVolume() * 100, 1)));

            if (api.Player_GetMute())
            {
                api.Player_SetMute(false);
            }
        }

        /// <summary>
        /// Changes the player shuffle state. If the StateAction is Toggle then the current state is switched with it's opposite,
        /// if it is State the current state is dispatched with an Event.
        /// </summary>
        /// <param name="action"></param>
        public void RequestShuffleState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                api.Player_SetShuffle(!api.Player_GetShuffle());
            }
            
            SendSocketMessage(Constants.PlayerShuffle, Constants.Reply, api.Player_GetShuffle());
        }

        /// <summary>
        /// Changes the player mute state. If the StateAction is Toggle then the current state is switched with it's opposite,
        /// if it is State the current state is dispatched with an Event.
        /// </summary>
        /// <param name="action"></param>
        public void RequestMuteState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                api.Player_SetMute(!api.Player_GetMute());
            }
            
            SendSocketMessage(Constants.PlayerMute, Constants.Reply, api.Player_GetMute());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public void RequestScrobblerState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                api.Player_SetScrobbleEnabled(!api.Player_GetScrobbleEnabled());
            }
            
            SendSocketMessage(Constants.PlayerScrobble, Constants.Reply, api.Player_GetScrobbleEnabled());
        }

        /// <summary>
        /// If the action equals toggle then it changes the repeat state, in any other case
        /// it just returns the current value of the repeat.
        /// </summary>
        /// <param name="action">toggle or state</param>
        /// <returns>Repeat state: None, All, One</returns>
        public void RequestRepeatState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                switch (api.Player_GetRepeat())
                {
                    case RepeatMode.None:
                        api.Player_SetRepeat(RepeatMode.All);
                        break;
                    case RepeatMode.All:
                        api.Player_SetRepeat(RepeatMode.None);
                        break;
                    case RepeatMode.One:
                        api.Player_SetRepeat(RepeatMode.None);
                        break;
                }
            }
            SendSocketMessage(Constants.PlayerRepeat, Constants.Reply, api.Player_GetRepeat());
        }

        /// <summary>
        /// It gets the 100 first tracks of the playlist and returns them in an XML formated String without a root element.
        /// </summary>
        /// <param name="clientProtocolVersion"> </param>
        /// <param name="clientId"> </param>
        /// <returns>XML formated string without root element</returns>
        public void RequestNowPlayingList(double clientProtocolVersion, string clientId)
        {
            api.NowPlayingList_QueryFiles(null);

            List<NowPlayingListTrack> trackList = new List<NowPlayingListTrack>();
            int position = 1;
            while (position <= UserSettings.Instance.NowPlayingListLimit)
            {
                string playListTrack = api.NowPlayingList_QueryGetNextFile();
                if (String.IsNullOrEmpty(playListTrack))
                    break;

                string artist = api.Library_GetFileTag(playListTrack, MetaDataType.Artist);
                string title = api.Library_GetFileTag(playListTrack, MetaDataType.TrackTitle);

                if (String.IsNullOrEmpty(artist))
                {
                    artist = "Unknown Artist";
                }

                if (String.IsNullOrEmpty(title))
                {
                    int index = playListTrack.LastIndexOf('\\');
                    title = playListTrack.Substring(index + 1);
                }

                trackList.Add(
                    new NowPlayingListTrack(artist, title, position));
                position++;
            }

            SendSocketMessage(Constants.NowPlayingList, Constants.Reply, trackList, clientId);
        }

        /// <summary>
        /// If the given rating string is not null or empty and the value of the string is a float number in the [0,5]
        /// the function will set the new rating as the current index's new index rating. In any other case it will
        /// just return the rating for the current index.
        /// </summary>
        /// <param name="rating">New Track Rating</param>
        /// <param name="clientId"> </param>
        /// <returns>Track Rating</returns>
        public void RequestTrackRating(string rating, string clientId)
        {
            try
            {
                char a = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                rating = rating.Replace('.', a);
                float fRating;
                if (!Single.TryParse(rating, out fRating))
                {
                    fRating = -1;
                }
                if (fRating >= 0  && fRating <= 5)
                {
                    api.Library_SetFileTag(api.NowPlaying_GetFileUrl(), MetaDataType.Rating, fRating.ToString());
                    api.Library_CommitTagsToFile(api.NowPlaying_GetFileUrl());
                    api.Player_GetShowRatingTrack();
                    api.MB_RefreshPanels();
                }
                rating = api.Library_GetFileTag(
                    api.NowPlaying_GetFileUrl(), MetaDataType.Rating).Replace(a, '.');
                
                SendSocketMessage(Constants.NowPlayingRating, Constants.Reply, rating);
            }
            catch (Exception e)
            {
#if DEBUG
                ErrorHandler.LogError(e);
#endif
            }
        }

        /// <summary>
        /// Requests the Now Playing index lyrics. If the lyrics are available then they are dispatched along with
        /// and event. If not, and the ApiRevision is equal or greater than r17 a request for the downloaded lyrics
        /// is initiated. The lyrics are dispatched along with and event when ready.
        /// </summary>
        public void RequestNowPlayingTrackLyrics()
        {
            if (!String.IsNullOrEmpty(api.NowPlaying_GetLyrics()))
            {
                SendSocketMessage(Constants.NowPlayingLyrics, Constants.Reply, api.NowPlaying_GetLyrics());
            }
            else if (api.ApiRevision >= 17)
            {
                string lyrics = api.NowPlaying_GetDownloadedLyrics();
                SendSocketMessage(Constants.NowPlayingLyrics, Constants.Reply, 
                    !String.IsNullOrEmpty(lyrics) ?
                    lyrics :
                    "Retrieving Lyrics");
            }
            else
            {
                SendSocketMessage(Constants.NowPlayingLyrics, Constants.Reply, "Lyrics Not Found");
            }
        }

        /// <summary>
        /// Requests the Now Playing Track Cover. If the cover is available it is dispatched along with an event.
        /// If not, and the ApiRevision is equal or greater than r17 a request for the downloaded artwork is
        /// initiated. The cover is dispatched along with an event when ready.
        /// </summary>
        public void RequestNowPlayingTrackCover()
        {
            if (!String.IsNullOrEmpty(api.NowPlaying_GetArtwork()))
            {
                EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange,
                                                    api.NowPlaying_GetArtwork(), "",
                                                    api.NowPlaying_GetFileTag(MetaDataType.Album)));
            }
            else if (api.ApiRevision >= 17)
            {
                string cover = api.NowPlaying_GetDownloadedArtwork();
                if (!String.IsNullOrEmpty(cover))
                {
                    EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange, cover, "",
                                                        api.NowPlaying_GetFileTag(MetaDataType.Album)));
                }
            }
            else
            {
                EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange, String.Empty, "",
                                                    api.NowPlaying_GetFileTag(MetaDataType.Album)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public void RequestPlayPosition(string request)
        {
            if (!request.Contains("status"))
            {
                int newPosition;
                if (Int32.TryParse(request, out newPosition))
                {
                    api.Player_SetPosition(newPosition);
                }
            }
            int currentPosition = api.Player_GetPosition();
            int totalDuration = api.NowPlaying_GetDuration();

            var position = new
                {
                    current = currentPosition,
                    total = totalDuration
                };
            
            SendSocketMessage(Constants.NowPlayingPosition, Constants.Reply, position);
        }

        /// <summary>
        /// Searches in the Now playing list for the index specified and plays it.
        /// </summary>
        /// <param name="index">The index to play</param>
        /// <returns></returns>
        public void NowPlayingPlay(string index)
        {
            bool result = false;
            int trackIndex;
            if (Int32.TryParse(index, out trackIndex))
            {
                api.NowPlayingList_QueryFiles(null);
                string trackToPlay = String.Empty;
                int lTrackIndex = 0;
                while (trackIndex != lTrackIndex)
                {
                    trackToPlay = api.NowPlayingList_QueryGetNextFile();
                    lTrackIndex++;
                }
                if (!String.IsNullOrEmpty(trackToPlay))
                    result = api.NowPlayingList_PlayNow(trackToPlay);
            }
            
            SendSocketMessage(Constants.NowPlayingListPlay, Constants.Reply, result);
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="clientId"></param>
        public void NowPlayingListRemoveTrack(int index, string clientId)
        {
            var reply = new
            {
                success = api.NowPlayingList_RemoveAt(index),
                index
            };
            SendSocketMessage(Constants.NowPlayingListRemove, Constants.Reply,reply, clientId);
        }

        /// <summary>
        /// This function requests or changes the AutoDJ functionality's state.
        /// </summary>
        /// <param name="action">
        /// The action can be either toggle or state.
        /// </param>
        public void RequestAutoDjState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                if (!api.Player_GetAutoDjEnabled())
                {
                    api.Player_StartAutoDj();
                }
                else
                {
                    api.Player_EndAutoDj();
                }
            }
            SendSocketMessage(Constants.PlayerAutoDj, Constants.Reply, api.Player_GetAutoDjEnabled());
        }

        /// <summary>
        /// This function is used to change the playing index's last.fm love rating.
        /// </summary>
        /// <param name="action">
        /// The action can be either love, or ban.
        /// </param>
        public void RequestLoveStatus(string action)
        {
            IntPtr hwnd = api.MB_GetWindowHandle();
            Form MB = (Form) Control.FromHandle(hwnd);

            if (action.Equals("toggle", StringComparison.OrdinalIgnoreCase))
            {
                if (GetLfmStatus() == LastfmStatus.Love || GetLfmStatus() == LastfmStatus.Ban)
                {
                    MB.Invoke(new MethodInvoker(SetLfmNormalStatus));
                }
                else
                {
                    MB.Invoke(new MethodInvoker(SetLfmLoveStatus));    
                }
            } 
            else if (action.Equals("love", StringComparison.OrdinalIgnoreCase))
            {
                MB.Invoke(new MethodInvoker(SetLfmLoveStatus));
            }
            else if (action.Equals("ban", StringComparison.OrdinalIgnoreCase))
            {
                MB.Invoke(new MethodInvoker(SetLfmLoveBan));
            }
            
            SendSocketMessage(Constants.NowPlayingLfmRating, Constants.Reply, GetLfmStatus());
        }

        private void SetLfmNormalStatus()
        {
            api.Library_SetFileTag(
                    api.NowPlaying_GetFileUrl(), MetaDataType.RatingLove, "lfm");
        }

        private void SetLfmLoveStatus()
        {
            api.Library_SetFileTag(
                    api.NowPlaying_GetFileUrl(), MetaDataType.RatingLove, "Llfm");
        }

        private void SetLfmLoveBan()
        {
            api.Library_SetFileTag(
                    api.NowPlaying_GetFileUrl(), MetaDataType.RatingLove, "Blfm");
        }

        private LastfmStatus GetLfmStatus()
        {
            LastfmStatus lastfmStatus;
            string apiReply = api.NowPlaying_GetFileTag(MetaDataType.RatingLove);
            if (apiReply.Equals("L") || apiReply.Equals("lfm") || apiReply.Equals("Llfm"))
            {
                lastfmStatus = LastfmStatus.Love;
            }
            else if (apiReply.Equals("B") || apiReply.Equals("Blfm"))
            {
                lastfmStatus = LastfmStatus.Ban;
            }
            else
            {
                lastfmStatus = LastfmStatus.Normal;
            }
            return lastfmStatus;
        }

        /// <summary>
        /// The function checks the MusicBee api and gets all the available playlist urls.
        /// </summary>
        public void GetAvailablePlaylists()
        {
            api.Playlist_QueryPlaylists();
            string playlistUrl;
            List<Playlist> availablePlaylists = new List<Playlist>();
            while (true)
            {
                playlistUrl = api.Playlist_QueryGetNextPlaylist();
                if (String.IsNullOrEmpty(playlistUrl)) break;
                string name = api.Playlist_GetName(playlistUrl);
                string[] files = { };
                api.Playlist_QueryFilesEx(playlistUrl, ref files);
                Playlist playlist = new Playlist(name, files.Count(), playlistUrl);
                availablePlaylists.Add(playlist);
            }

            SendSocketMessage(Constants.PlaylistList, Constants.Reply, availablePlaylists);
        }

        /// <summary>
        /// Given the url of a playlist and the id of a client the method sends a message to the specified client
        /// including the tracks in the specified playlist.
        /// </summary>
        /// <param name="url">The playlist url</param>
        /// <param name="clientId">The id of the client</param>
        public void GetTracksForPlaylist(string url, string clientId)
        {

            string[] trackUrlList = {};

            if (!api.Playlist_QueryFilesEx(url, ref trackUrlList))
            {
                return;
            }

            List<Track> playlistTracks = new List<Track>();

            foreach (string trackUrl in trackUrlList)
            {
                string artist = api.Library_GetFileTag(trackUrl, MetaDataType.Artist);
                string track = api.Library_GetFileTag(trackUrl, MetaDataType.TrackTitle);
                Track curTrack = new Track(artist, track, trackUrl);
                playlistTracks.Add(curTrack);
            }

            SendSocketMessage(Constants.PlaylistGetFiles, Constants.Reply, playlistTracks);
        }

        /// <summary>
        /// Given the url of a playlist it plays the specified playlist.
        /// </summary>
        /// <param name="url">The playlist url</param>
        public void RequestPlaylistPlayNow(string url)
        {
            SendSocketMessage(Constants.PlaylistPlayNow, Constants.Reply, api.Playlist_PlayNow(url));
        }

        /// <summary>
        /// Given the url of the playlist and the index of a index it removes the specified index,
        /// from the playlist.
        /// </summary>
        /// <param name="url">The url of th playlist</param>
        /// <param name="index">The index of the index to remove</param>
        public void RequestPlaylistTrackRemove(string url,int index)
        {
            bool success = api.Playlist_RemoveAt(url, index);
            SendSocketMessage(Constants.PlaylistRemove, Constants.Reply, success);
        }

        public void RequestPlaylistCreate(string client, string name, MetaTag tag, string query, string[] files)
        {
            if (tag != MetaTag.title)
            {
                files = GetUrlsForTag(tag, query);
            }
            string url = api.Playlist_CreatePlaylist(String.Empty, name, files);
            SendSocketMessage(Constants.PlaylistCreate, Constants.Reply, url);
        }

        public void RequestPlaylistMove(string clientId,string src, int from, int to)
        {
            bool success;
            int[] aFrom = { from };
            int dIn;
            if (from > to)
            {
                dIn = to - 1;
            }
            else
            {
                dIn = to;
            }

            success = api.Playlist_MoveFiles(src, aFrom, dIn);

            var reply = new
            {
                success,
                from,
                to
            };

            SendSocketMessage(Constants.PlaylistMove, Constants.Reply, reply, clientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        public void RequestPlayerStatus(string clientId)
        {
            var status = new
            {
                playerrepeat = api.Player_GetRepeat().ToString(),
                playermute = api.Player_GetMute(),
                playershuffle = api.Player_GetShuffle(),
                scrobbler = api.Player_GetScrobbleEnabled(),
                playerstate = api.Player_GetPlayState().ToString(),
                playervolume =
                    ((int) Math.Round(api.Player_GetVolume()*100, 1)).ToString(
                        CultureInfo.InvariantCulture)
            };


            SendSocketMessage(Constants.PlayerStatus, Constants.Reply, status, clientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        public void RequestTrackInfo(string clientId)
        {
            SendSocketMessage(Constants.NowPlayingTrack, Constants.Reply, GetTrackInfo(), clientId);
        }


        /// <summary>
        /// Moves a index of the now playing list to a new position.
        /// </summary>
        /// <param name="clientId">The Id of the client that initiated the request</param>
        /// <param name="from">The initial position</param>
        /// <param name="to">The final position</param>
        public void RequestNowPlayingMove(string clientId, int from, int to)
        {
            bool result = false;
            int[] aFrom = {from};
            int dIn;
            if (from > to)
            {
                dIn = to - 1;
            }
            else
            {
                dIn = to;
            }
            result = api.NowPlayingList_MoveFiles(aFrom, dIn);

            var reply = new
            {
                success = result, from, to
            };

            SendSocketMessage(Constants.NowPlayingListMove, Constants.Reply, reply, clientId);
        }

        private string XmlFilter(string[] tags, string query, bool isStrict)
        {
            XElement filter = new XElement("Source",
                                           new XAttribute("Type", 1));

            XElement conditions = new XElement("Conditions",
                                               new XAttribute("CombineMethod", "Any"));
            foreach (string tag in tags)
            {
                XElement condition = new XElement("Condition",
                                                  new XAttribute("Field", tag),
                                                  new XAttribute("Comparison", isStrict ? "Is" : "Contains"),
                                                  new XAttribute("Value", query));
                conditions.Add(condition);
            }
            filter.Add(conditions);

            return filter.ToString();
        }

        /// <summary>
        /// Calls the API to get albums matching the specified parameter. Fires an event with the message in JSON format.
        /// </summary>
        /// <param name="albumName">Is used to filter through the data.</param>
        /// <param name="clientId">The client that initiated the call. (Should also be the only one to receive the data.</param>
        public void LibrarySearchAlbums(string albumName, string clientId)
        {
            List<Album> albumList = new List<Album>();

            if (api.Library_QueryLookupTable("album", "albumartist" + '\0' + "album", XmlFilter(new[] {"Album"}, albumName, false)))
            {
                try
                {
                    foreach (string entry in new List<string>(api.Library_QueryGetLookupTableValue(null).Split(new[] {"\0\0"}, StringSplitOptions.None)))
                    {
                        if (String.IsNullOrEmpty(entry)) continue;
                        string[] albumInfo = entry.Split('\0');
                        if (albumInfo.Length < 2) continue;

                        Album current = albumInfo.Length == 3
                                            ? new Album(albumInfo[1], albumInfo[2])
                                            : new Album(albumInfo[0], albumInfo[1]);
                        if (current.album.IndexOf(albumName, StringComparison.OrdinalIgnoreCase) < 0) continue;

                        if (!albumList.Contains(current))
                        {
                            albumList.Add(current);
                        }
                        else
                        {
                            albumList.ElementAt(albumList.IndexOf(current)).IncreaseCount();
                        }
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            api.Library_QueryLookupTable(null, null, null);
            
            SendSocketMessage(Constants.LibrarySearchAlbum, Constants.Reply, albumList, clientId);
            albumList = null;
        }
        
        /// <summary>
        /// Used to get all the albums by the specified artist.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="clientId"></param>
        public void LibraryGetArtistAlbums(string artist, string clientId)
        {
            List<Album> albumList = new List<Album>();
            if (api.Library_QueryFiles(XmlFilter(new[] {"ArtistPeople"}, artist, true)))
            {
                while (true)
                {
                    string currentFile = api.Library_QueryGetNextFile();
                    if (String.IsNullOrEmpty(currentFile)) break;
                    Album current = new Album(api.Library_GetFileTag(currentFile, MetaDataType.AlbumArtist),
                                              api.Library_GetFileTag(currentFile, MetaDataType.Album));
                    if (!albumList.Contains(current))
                    {
                        albumList.Add(current);
                    }
                    else
                    {
                        albumList.ElementAt(albumList.IndexOf(current)).IncreaseCount();
                    }
                }
            }
            SendSocketMessage(Constants.LibraryArtistAlbums, Constants.Reply, albumList, clientId);
            albumList = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="clientId"></param>
        public void LibrarySearchArtist(string artist, string clientId)
        {
            List<Artist> artistList = new List<Artist>();
            if (api.Library_QueryLookupTable("artist", "count",
                                                        XmlFilter(new[] {"ArtistPeople"}, artist, false)))
            {
                foreach (string entry in api.Library_QueryGetLookupTableValue(null).Split(new[] {"\0\0"}, StringSplitOptions.None))
                {
                    string[] artistInfo = entry.Split(new[] { '\0' });
                    artistList.Add(new Artist(artistInfo[0], Int32.Parse(artistInfo[1])));
                }
            }

            api.Library_QueryLookupTable(null, null, null);

            SendSocketMessage(Constants.LibrarySearchArtist, Constants.Reply, artistList, clientId);
            artistList = null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="genre"></param>
        /// <param name="clientId"></param>
        public void LibraryGetGenreArtists(string genre, string clientId)
        {
            List<Artist> artistList = new List<Artist>();

            if (api.Library_QueryLookupTable("artist", "count", XmlFilter(new[] {"Genre"}, genre, true)))
            {
                foreach (string entry in api.Library_QueryGetLookupTableValue(null).Split(new[] {"\0\0"}, StringSplitOptions.None))
                {
                    string[] artistInfo = entry.Split(new[] {'\0'});
                    artistList.Add(new Artist(artistInfo[0], Int32.Parse(artistInfo[1])));        
                }
            }

            api.Library_QueryLookupTable(null, null, null);
            SendSocketMessage(Constants.LibraryGenreArtists, Constants.Reply, artistList, clientId);

            artistList = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="genre"></param>
        /// <param name="clientId"></param>
        public void LibrarySearchGenres(string genre, string clientId)
        {
            List<Genre> genreList = new List<Genre>();
            if (api.Library_QueryLookupTable("genre", "count",
                                                        XmlFilter(new[] {"Genre"}, genre, false)))
            {
                foreach (string entry in api.Library_QueryGetLookupTableValue(null).Split(new[] {"\0\0"}, StringSplitOptions.None))
                {
                    string[] genreInfo = entry.Split(new[] {'\0'}, StringSplitOptions.None);
                    genreList.Add(new Genre(genreInfo[0], Int32.Parse(genreInfo[1])));   
                }
            }
            api.Library_QueryLookupTable(null, null, null);

            SendSocketMessage(Constants.LibrarySearchGenre, Constants.Reply, genreList, clientId);

            genreList = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="clientId"></param>
        public void LibrarySearchTitle(string title, string clientId)
        {
            List<Track> tracks = new List<Track>();
            if (api.Library_QueryFiles(XmlFilter(new[] {"Title"}, title, false)))
            {
                while (true)
                {
                    string currentTrack = api.Library_QueryGetNextFile();
                    if (String.IsNullOrEmpty(currentTrack)) break;

                    int trackNumber = 0;
                    Int32.TryParse(api.Library_GetFileTag(currentTrack, MetaDataType.TrackNo), out trackNumber);

                    tracks.Add(new Track(api.Library_GetFileTag(currentTrack, MetaDataType.Artist),
                                         api.Library_GetFileTag(currentTrack, MetaDataType.TrackTitle),
                                         trackNumber, currentTrack));
                }
            }

            api.Library_QueryLookupTable(null, null, null);

            SendSocketMessage(Constants.LibrarySearchTitle, Constants.Reply, tracks, clientId);
            tracks = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="album"></param>
        /// <param name="client"></param>
        public void LibraryGetAlbumTracks(string album, string client)
        {
            List<Track> trackList = new List<Track>();
            if (api.Library_QueryFiles(XmlFilter(new[] {"Album"}, album, true)))
            { 
                while (true)
                {
                    string currentTrack = api.Library_QueryGetNextFile();
                    if (String.IsNullOrEmpty(currentTrack)) break;

                    int trackNumber = 0;
                    Int32.TryParse(api.Library_GetFileTag(currentTrack, MetaDataType.TrackNo), out trackNumber);
                    string src = Convert.ToBase64String(Encoding.UTF8.GetBytes(currentTrack));

                    trackList.Add(new Track(api.Library_GetFileTag(currentTrack, MetaDataType.Artist),
                                              api.Library_GetFileTag(currentTrack, MetaDataType.TrackTitle), trackNumber, src));
                }
                trackList.Sort();
            }

            SendSocketMessage(Constants.LibraryAlbumTracks, Constants.Reply, trackList, client);

            trackList = null;
        }

        private string[] GetUrlsForTag(MetaTag tag, string query)
        {
            string filter = String.Empty;
            string[] tracks = {};
            switch (tag)
            {
                case MetaTag.artist:
                    filter = XmlFilter(new[] { "ArtistPeople" }, query, true);
                    break;
                case MetaTag.album:
                    filter = XmlFilter(new[] { "Album" }, query, true);
                    break;
                case MetaTag.genre:
                    filter = XmlFilter(new[] { "Genre" }, query, true);
                    break;
            }

            api.Library_QueryFilesEx(filter, ref tracks);

            return tracks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="tag"></param>
        /// <param name="query"></param>
        public void RequestQueueFiles(QueueType queue, MetaTag tag, string query)
        {
            string filter = String.Empty;
            List<string> trackList = new List<string>();
            bool loop = true;
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
                case MetaTag.title:
                    trackList.Add(query);
                    loop = false;
                    break;
                case MetaTag.none:
                    return;
                default:
                    return;
            }

            if (trackList.Count == 0)
            {
                api.Library_QueryFiles(filter);
                while (loop)
                {
                    string current = api.Library_QueryGetNextFile();
                    if (String.IsNullOrEmpty(current)) break;
                    trackList.Add(current);
                }
            }

            if (queue == QueueType.Next)
            {
                api.NowPlayingList_QueueFilesNext(trackList.ToArray());
            }
            else if (queue == QueueType.Last)
            {
                api.NowPlayingList_QueueFilesLast(trackList.ToArray());
            }
            else if (queue == QueueType.PlayNow)
            {
                api.NowPlayingList_Clear();
                api.NowPlayingList_QueueFilesLast(trackList.ToArray());
                api.NowPlayingList_PlayNow(trackList[0]);
            }
        }

        /// <summary>
        /// Takes a given query string and searches the Now Playing list for any index with a matching title or artist.
        /// The title is checked first.
        /// </summary>
        /// <param name="query">The string representing the query</param>
        /// <param name="clientId">Client</param>
        public void NowPlayingSearch(string query, string clientId)
        {
            bool result = false;
            api.NowPlayingList_QueryFiles(XmlFilter(new[] {"ArtistPeople", "Title"}, query, false));

            while (true)
            {
                string currentTrack = api.NowPlayingList_QueryGetNextFile();
                if (String.IsNullOrEmpty(currentTrack)) break;
                string artist = api.Library_GetFileTag(currentTrack, MetaDataType.Artist);
                string title = api.Library_GetFileTag(currentTrack, MetaDataType.TrackTitle);

                if (title.IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0 &&
                    artist.IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0) continue;
                result = api.NowPlayingList_PlayNow(currentTrack);
                break;
            }

            SendSocketMessage(Constants.NowPlayingListSearch, Constants.Reply, result, clientId);
        }

        public void SyncCheckForChanges(string[] cachedFiles ,DateTime lastSync)
        {
            string[] newFiles = {};
            string[] deletedFiles = {};
            string[] updatedFiles ={};

            api.Library_GetSyncDelta(cachedFiles, lastSync, LibraryCategory.Music,
                ref newFiles, ref updatedFiles, ref deletedFiles);

            var jsonData = new
            {
                type = "partial",
                update = updatedFiles,
                deleted = deletedFiles,
                newfiles = newFiles
            };

            SendSocketMessage(Constants.LibrarySync, Constants.Reply, jsonData);
        }

        public void SyncGetFilenames(string clientId)
        {
            string[] files = {};
            api.Library_QueryFilesEx(String.Empty, ref files);
            var jsonData = new
            {
                type = "full",
                payload = files.Length
            };

            SendSocketMessage(Constants.LibrarySync, Constants.Reply, jsonData, clientId);
        }

        public void SyncGetCover(string hash, string clientId)
        {
            var cachedEntry = mHelper.GetEntryByHash(hash);
            
            var payload = new
            {
                hash = cachedEntry.CoverHash,
                image = Utilities.GetCachedImage(cachedEntry.CoverHash)
            };

            var jsonData = new
            {
                type = "cover",
                file = hash,
                hash = true,
                payload
            };

            SendSocketMessage(Constants.LibrarySync, Constants.Reply, jsonData, clientId);
        }

        public void BuildCache()
        {
            string[] files = {};
            api.Library_QueryFilesEx(String.Empty, ref files);

            foreach (var file in files)
            {    
                var fileHash = Utilities.Sha1Hash(file);
                mHelper.CacheEntry(fileHash,file);
            }
        }

        public void BuildCoverCache()
        {
            foreach (var entry in mHelper.GetCachedFiles())
            {
                var cover = api.Library_GetArtwork(entry.Filepath, 0);
                entry.CoverHash = Utilities.CacheImage(cover);
                mHelper.UpdateCoverHash(entry.Hash, entry.CoverHash);
            }   
        }

        private void SendSocketMessage(string command, string type, object data, string client = "all")
        {
            SocketMessage msg = new SocketMessage(command, type, data);
            MessageEvent mEvent = new MessageEvent(EventType.ReplyAvailable, msg.toJsonString());
            EventBus.FireEvent(mEvent);
        }

        public void SyncGetMetaData(int index, string client)
        {
            var entry = mHelper.GetEntryAt(index);
            var file = entry.Filepath;
            var meta = new MetaData {hash = entry.Hash, cover_hash = entry.CoverHash};

            if (MusicBeeVersion.v2_2 == api.MusicBeeVersion)
            {
                
                meta.artist = api.Library_GetFileTag(file, MetaDataType.Artist);
                meta.album_artist = api.Library_GetFileTag(file, MetaDataType.AlbumArtist);
                meta.album = api.Library_GetFileTag(file, MetaDataType.Album);
                meta.title = api.Library_GetFileTag(file, MetaDataType.TrackTitle);
                meta.genre = api.Library_GetFileTag(file, MetaDataType.Genre);
                meta.year = api.Library_GetFileTag(file, MetaDataType.Year);
                meta.track_no = api.Library_GetFileTag(file, MetaDataType.TrackNo);
            
            }
            else
            {
                MetaDataType[] types =
                {
                    MetaDataType.Artist,
                    MetaDataType.AlbumArtist,
                    MetaDataType.Album,
                    MetaDataType.TrackTitle,
                    MetaDataType.Genre,
                    MetaDataType.Year,
                    MetaDataType.TrackNo
                };
                var i = 0;
                string[] tags = {};
                api.Library_GetFileTags(file, types, ref tags);
                meta.artist = tags[i++];
                meta.album_artist = tags[i++]; 
                meta.album = tags[i++];
                meta.title = tags[i++];
                meta.genre = tags[i++];
                meta.year = tags[i++];
                meta.track_no = tags[i];
            }
//            string[] urls = {};
//            string url = String.Empty;
//            api.Library_GetArtistPictureUrls(meta.artist, false, ref urls);
//            
//            if (urls.Length > 0)
//            {
//                string pattern = "^http";
//                foreach (var location in urls)
//                {
//                    if (!Regex.IsMatch(location, pattern)) continue;
//                    url = location;
//                    break;
//                }
//            }
//            meta.artist_image_url = url;

            SendSocketMessage(Constants.LibrarySync, Constants.Reply, meta, client);
        }


        public void DumpDb()
        {
//            using (var db = Db4oEmbedded.OpenFile(mStoragePath + "\\cache.db"))
//            {
//                var data = from LibraryData ldata in db select ldata;
//                Debug.WriteLine("Total entries stored {0}", data.Count());
//                foreach (var entry in data)
//                {
//                    Debug.WriteLine(entry.Dump());
//                }
//            }
        }
    }
}