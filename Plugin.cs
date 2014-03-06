namespace MusicBeePlugin
{
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
    public partial class Plugin : Messenger
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
        public static Plugin Instance
        {
            get { return _selfInstance; }
        }

        private static Plugin _selfInstance;
        private InfoWindow _mWindow;

#if DEBUG
        private DebugTool _dTool;
#endif
        private string _mStoragePath;

        public SyncModule SyncModule { get; private set; }

        public PlaylistModule PlaylistModule { get; private set; }


        /// <summary>
        /// This function initialized the Plugin.
        /// </summary>
        /// <param name="apiInterfacePtr"></param>
        /// <returns></returns>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            _selfInstance = this;
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

            ErrorHandler.SetLogFilePath(_api.Setting_GetPersistentStoragePath());
            _mStoragePath = _api.Setting_GetPersistentStoragePath() + "\\mb_remote";

            _api.MB_AddMenuItem("mnuTools/MusicBee Remote", "Information Panel of the MusicBee Remote",
                                          MenuItemClicked);

            EventBus.FireEvent(new MessageEvent(EventType.ActionSocketStart));
            EventBus.FireEvent(new MessageEvent(EventType.InitializeModel));
            EventBus.FireEvent(new MessageEvent(EventType.StartServiceBroadcast));
            EventBus.FireEvent(new MessageEvent(EventType.ShowFirstRunDialog));

            _positionUpdateTimer = new Timer(20000);
            _positionUpdateTimer.Elapsed += PositionUpdateTimerOnElapsed;
            _positionUpdateTimer.Enabled = true;
            
            SyncModule = new SyncModule(_api, _mStoragePath);
            PlaylistModule = new PlaylistModule(_api, _mStoragePath);

#if DEBUG
            _api.MB_AddMenuItem("mnuTools/MBRC Debug Tool", "DebugTool",
                                          DisplayDebugWindow);
#endif

            SyncModule.CheckCacheState();

            return _about;
        }

        private void PositionUpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_api.Player_GetPlayState() == PlayState.Playing)
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
            string settingsFolder = _api.Setting_GetPersistentStoragePath + "\\mb_remote";
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
                    RequestNowPlayingTrackCover();
                    RequestTrackRating(String.Empty, String.Empty);
                    RequestLoveStatus("status");
                    RequestNowPlayingTrackLyrics();
                    RequestPlayPosition("status");
                    SendSocketMessage(Constants.NowPlayingTrack, Constants.Message, GetTrackInfo());
                    break;
                case NotificationType.VolumeLevelChanged:
                    SendSocketMessage(Constants.PlayerVolume, Constants.Message,((int) Math.Round(_api.Player_GetVolume()*100,1)));
                    break;
                case NotificationType.VolumeMuteChanged:
                    SendSocketMessage(Constants.PlayerMute, Constants.Message, _api.Player_GetMute());
                    break;
                case NotificationType.PlayStateChanged:
                    SendSocketMessage(Constants.PlayerState, Constants.Message,_api.Player_GetPlayState());
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
                                                            _api.NowPlaying_GetDownloadedArtwork(), "",
                                                            _api.NowPlaying_GetFileTag(MetaDataType.Album)));
                    }
                    break;
                case NotificationType.NowPlayingListChanged:
                    SendSocketMessage(Constants.NowPlayingListChanged, Constants.Message, true);
                    break;
                case NotificationType.PlayerRepeatChanged :
                    var repeat = _api.Player_GetRepeat();
                    SendSocketMessage(Constants.PlayerRepeat, Constants.Message, repeat);
                    break;
                case NotificationType.PlayerShuffleChanged:
                    var shuffle = _api.Player_GetShuffle();
                    SendSocketMessage(Constants.PlayerShuffle, Constants.Message, shuffle);
                    break;
                case NotificationType.PlayerScrobbleChanged:
                    var scrobble = _api.Player_GetScrobbleEnabled();
                    SendSocketMessage(Constants.PlayerScrobble, Constants.Message, scrobble);
                    break;
                case NotificationType.AutoDjStarted:
                    SendSocketMessage(Constants.PlayerAutoDj, Constants.Reply, true);
                    break;
                case NotificationType.AutoDjStopped:
                    SendSocketMessage(Constants.PlayerAutoDj, Constants.Reply, false);
                    break;
            }
        }

        private NowPlayingTrack GetTrackInfo()
        {
            var nowPlayingTrack = new NowPlayingTrack
            {
                Artist = _api.NowPlaying_GetFileTag(MetaDataType.Artist),
                Album = _api.NowPlaying_GetFileTag(MetaDataType.Album),
                Year = _api.NowPlaying_GetFileTag(MetaDataType.Year)
            };
            nowPlayingTrack.SetTitle(_api.NowPlaying_GetFileTag(MetaDataType.TrackTitle),
                                     _api.NowPlaying_GetFileUrl());
            return nowPlayingTrack;
        }

        /// <summary>
        /// When called plays the next index.
        /// </summary>
        /// <returns></returns>
        public void RequestNextTrack(string clientId)
        {
            SendSocketMessage(Constants.PlayerNext, Constants.Reply, _api.Player_PlayNextTrack());
        }

        /// <summary>
        /// When called stops the playback.
        /// </summary>
        /// <returns></returns>
        public void RequestStopPlayback(string clientId)
        {
            SendSocketMessage(Constants.PlayerStop, Constants.Reply, _api.Player_Stop());
        }

        /// <summary>
        /// When called changes the play/pause state or starts playing a index if the status is stopped.
        /// </summary>
        /// <returns></returns>
        public void RequestPlayPauseTrack(string clientId)
        {
            SendSocketMessage(Constants.PlayerPlayPause, Constants.Reply, _api.Player_PlayPause());
        }

        /// <summary>
        /// When called plays the previous index.
        /// </summary>
        /// <returns></returns>
        public void RequestPreviousTrack(string clientId)
        {
            SendSocketMessage(Constants.PlayerPrevious, Constants.Reply, _api.Player_PlayPreviousTrack());
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
                _api.Player_SetVolume((float) volume/100);
            }

            SendSocketMessage(Constants.PlayerVolume, Constants.Reply, ((int)Math.Round(_api.Player_GetVolume() * 100, 1)));

            if (_api.Player_GetMute())
            {
                _api.Player_SetMute(false);
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
                _api.Player_SetShuffle(!_api.Player_GetShuffle());
            }
            
            SendSocketMessage(Constants.PlayerShuffle, Constants.Reply, _api.Player_GetShuffle());
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
                _api.Player_SetMute(!_api.Player_GetMute());
            }
            
            SendSocketMessage(Constants.PlayerMute, Constants.Reply, _api.Player_GetMute());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public void RequestScrobblerState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                _api.Player_SetScrobbleEnabled(!_api.Player_GetScrobbleEnabled());
            }
            
            SendSocketMessage(Constants.PlayerScrobble, Constants.Reply, _api.Player_GetScrobbleEnabled());
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
                switch (_api.Player_GetRepeat())
                {
                    case RepeatMode.None:
                        _api.Player_SetRepeat(RepeatMode.All);
                        break;
                    case RepeatMode.All:
                        _api.Player_SetRepeat(RepeatMode.None);
                        break;
                    case RepeatMode.One:
                        _api.Player_SetRepeat(RepeatMode.None);
                        break;
                }
            }
            SendSocketMessage(Constants.PlayerRepeat, Constants.Reply, _api.Player_GetRepeat());
        }

        /// <summary>
        /// It gets the 100 first tracks of the playlist and returns them in an XML formated String without a root element.
        /// </summary>
        /// <param name="clientProtocolVersion"> </param>
        /// <param name="clientId"> </param>
        /// <returns>XML formated string without root element</returns>
        public void RequestNowPlayingList(double clientProtocolVersion, string clientId)
        {
            _api.NowPlayingList_QueryFiles(null);

            var trackList = new List<NowPlayingListTrack>();
            var position = 1;

            while (true)
            {
                var playListTrack = _api.NowPlayingList_QueryGetNextFile();
                if (String.IsNullOrEmpty(playListTrack))
                {
                    break;
                }

                var artist = _api.Library_GetFileTag(playListTrack, MetaDataType.Artist);
                var title = _api.Library_GetFileTag(playListTrack, MetaDataType.TrackTitle);

                if (String.IsNullOrEmpty(artist))
                {
                    artist = "Unknown Artist";
                }

                if (String.IsNullOrEmpty(title))
                {
                    var index = playListTrack.LastIndexOf('\\');
                    title = playListTrack.Substring(index + 1);
                }

                trackList.Add(new NowPlayingListTrack(artist, title, position, Utilities.Sha1Hash(playListTrack)));
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
                var a = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                rating = rating.Replace('.', a);
                float fRating;
                if (!Single.TryParse(rating, out fRating))
                {
                    fRating = -1;
                }
                if (fRating >= 0  && fRating <= 5)
                {
                    _api.Library_SetFileTag(_api.NowPlaying_GetFileUrl(),
                        MetaDataType.Rating,
                        fRating.ToString(CultureInfo.InvariantCulture));
                    _api.Library_CommitTagsToFile(_api.NowPlaying_GetFileUrl());
                    _api.Player_GetShowRatingTrack();
                    _api.MB_RefreshPanels();
                }
                rating = _api.Library_GetFileTag(
                    _api.NowPlaying_GetFileUrl(), MetaDataType.Rating).Replace(a, '.');
                
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
            if (!String.IsNullOrEmpty(_api.NowPlaying_GetLyrics()))
            {
                SendSocketMessage(Constants.NowPlayingLyrics, Constants.Reply, _api.NowPlaying_GetLyrics());
            }
            else if (_api.ApiRevision >= 17)
            {
                var lyrics = _api.NowPlaying_GetDownloadedLyrics();
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
            if (!String.IsNullOrEmpty(_api.NowPlaying_GetArtwork()))
            {
                EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange,
                                                    _api.NowPlaying_GetArtwork(), "",
                                                    _api.NowPlaying_GetFileTag(MetaDataType.Album)));
            }
            else if (_api.ApiRevision >= 17)
            {
                var cover = _api.NowPlaying_GetDownloadedArtwork();
                if (!String.IsNullOrEmpty(cover))
                {
                    EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange, cover, "",
                                                        _api.NowPlaying_GetFileTag(MetaDataType.Album)));
                }
            }
            else
            {
                EventBus.FireEvent(new MessageEvent(EventType.NowPlayingCoverChange, String.Empty, "",
                                                    _api.NowPlaying_GetFileTag(MetaDataType.Album)));
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
                    _api.Player_SetPosition(newPosition);
                }
            }
            var currentPosition = _api.Player_GetPosition();
            var totalDuration = _api.NowPlaying_GetDuration();

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
            var result = false;
            int trackIndex;
            if (Int32.TryParse(index, out trackIndex))
            {
                _api.NowPlayingList_QueryFiles(null);
                var trackToPlay = String.Empty;
                var lTrackIndex = 0;
                while (lTrackIndex != trackIndex)
                {
                    trackToPlay = _api.NowPlayingList_QueryGetNextFile();
                    lTrackIndex++;
                }
                if (!String.IsNullOrEmpty(trackToPlay))
                    result = _api.NowPlayingList_PlayNow(trackToPlay);
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
                success = _api.NowPlayingList_RemoveAt(index),
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
                if (!_api.Player_GetAutoDjEnabled())
                {
                    _api.Player_StartAutoDj();
                }
                else
                {
                    _api.Player_EndAutoDj();
                }
            }
            SendSocketMessage(Constants.PlayerAutoDj, Constants.Reply, _api.Player_GetAutoDjEnabled());
        }

        /// <summary>
        /// This function is used to change the playing index's last.fm love rating.
        /// </summary>
        /// <param name="action">
        /// The action can be either love, or ban.
        /// </param>
        public void RequestLoveStatus(string action)
        {
            var hwnd = _api.MB_GetWindowHandle();
            var mb = (Form) Control.FromHandle(hwnd);

            if (action.Equals("toggle", StringComparison.OrdinalIgnoreCase))
            {
                if (GetLfmStatus() == LastfmStatus.Love || GetLfmStatus() == LastfmStatus.Ban)
                {
                    mb.Invoke(new MethodInvoker(SetLfmNormalStatus));
                }
                else
                {
                    mb.Invoke(new MethodInvoker(SetLfmLoveStatus));    
                }
            } 
            else if (action.Equals("love", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(SetLfmLoveStatus));
            }
            else if (action.Equals("ban", StringComparison.OrdinalIgnoreCase))
            {
                mb.Invoke(new MethodInvoker(SetLfmLoveBan));
            }
            
            SendSocketMessage(Constants.NowPlayingLfmRating, Constants.Reply, GetLfmStatus());
        }

        private void SetLfmNormalStatus()
        {
            _api.Library_SetFileTag(
                    _api.NowPlaying_GetFileUrl(), MetaDataType.RatingLove, "lfm");
        }

        private void SetLfmLoveStatus()
        {
            _api.Library_SetFileTag(
                    _api.NowPlaying_GetFileUrl(), MetaDataType.RatingLove, "Llfm");
        }

        private void SetLfmLoveBan()
        {
            var fileUrl = _api.NowPlaying_GetFileUrl();
            _api.Library_SetFileTag(fileUrl, MetaDataType.RatingLove, "Blfm");
        }

        private LastfmStatus GetLfmStatus()
        {
            LastfmStatus lastfmStatus;
            var apiReply = _api.NowPlaying_GetFileTag(MetaDataType.RatingLove);
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
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        public void RequestPlayerStatus(string clientId)
        {
            var status = new
            {
                playerrepeat = _api.Player_GetRepeat().ToString(),
                playermute = _api.Player_GetMute(),
                playershuffle = _api.Player_GetShuffle(),
                scrobbler = _api.Player_GetScrobbleEnabled(),
                playerstate = _api.Player_GetPlayState().ToString(),
                playervolume =
                    ((int) Math.Round(_api.Player_GetVolume()*100, 1)).ToString(
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

            var result = _api.NowPlayingList_MoveFiles(aFrom, dIn);

            var reply = new
            {
                success = result, from, to
            };

            SendSocketMessage(Constants.NowPlayingListMove, Constants.Reply, reply, clientId);
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
            var filter = String.Empty;
            var trackList = new List<string>();
            var loop = true;
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
                _api.Library_QueryFiles(filter);
                if (loop)
                {
                    while (true)
                    {
                        var current = _api.Library_QueryGetNextFile();
                        if (String.IsNullOrEmpty(current)) break;
                        trackList.Add(current);
                    }    
                }
            }

            switch (queue)
            {
                case QueueType.Next:
                    _api.NowPlayingList_QueueFilesNext(trackList.ToArray());
                    break;
                case QueueType.Last:
                    _api.NowPlayingList_QueueFilesLast(trackList.ToArray());
                    break;
                case QueueType.PlayNow:
                    _api.NowPlayingList_Clear();
                    _api.NowPlayingList_QueueFilesLast(trackList.ToArray());
                    _api.NowPlayingList_PlayNow(trackList[0]);
                    break;
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
            var result = false;
            _api.NowPlayingList_QueryFiles(XmlFilter(new[] {"ArtistPeople", "Title"}, query, false));

            while (true)
            {
                var currentTrack = _api.NowPlayingList_QueryGetNextFile();
                if (String.IsNullOrEmpty(currentTrack)) break;
                var artist = _api.Library_GetFileTag(currentTrack, MetaDataType.Artist);
                var title = _api.Library_GetFileTag(currentTrack, MetaDataType.TrackTitle);

                if (title.IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0 &&
                    artist.IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0) continue;
                result = _api.NowPlayingList_PlayNow(currentTrack);
                break;
            }

            SendSocketMessage(Constants.NowPlayingListSearch, Constants.Reply, result, clientId);
        }
    }
}
