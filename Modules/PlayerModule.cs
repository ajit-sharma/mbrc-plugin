#region

using System;
using System.Globalization;
using MusicBeePlugin.AndroidRemote.Utilities;
using Ninject;

#endregion

namespace MusicBeePlugin.Modules
{
    public class PlayerModule
    {
        private readonly Plugin.MusicBeeApiInterface _api;

        public PlayerModule(Plugin.MusicBeeApiInterface api)
        {
            _api = api;
        }

        /// <summary>
        ///     When called plays the next index.
        /// </summary>
        /// <returns></returns>
        public bool RequestNextTrack(string clientId)
        {
            return _api.Player_PlayNextTrack();
        }

        /// <summary>
        ///     When called stops the playback.
        /// </summary>
        /// <returns></returns>
        public bool RequestStopPlayback(string clientId)
        {
            _api.Player_Stop();
            return true;
        }

        /// <summary>
        ///     When called changes the play/pause state or starts playing a index if the status is stopped.
        /// </summary>
        /// <returns></returns>
        public bool RequestPlayPauseTrack(string clientId)
        {
            return _api.Player_PlayPause();
        }

        /// <summary>
        ///     When called plays the previous index.
        /// </summary>
        /// <returns></returns>
        public bool RequestPreviousTrack(string clientId)
        {
            return _api.Player_PlayPreviousTrack();
        }

        /// <summary>
        ///     When called if the volume string is an integer in the range [0,100] it
        ///     changes the volume to the specific value and returns the new value.
        ///     In any other case it just returns the current value for the volume.
        /// </summary>
        /// <param name="volume"> </param>
        public void RequestVolumeChange(int volume)
        {
            if (volume >= 0)
            {
                _api.Player_SetVolume((float) volume/100);
            }

            //SendSocketMessage(Constants.PlayerVolume, Constants.Reply, ((int)Math.Round(_api.Player_GetVolume() * 100, 1)));

            if (_api.Player_GetMute())
            {
                _api.Player_SetMute(false);
            }
        }

        /// <summary>
        ///     Changes the player shuffle state. If the StateAction is Toggle then the current state is switched with it's
        ///     opposite,
        ///     if it is State the current state is dispatched with an Event.
        /// </summary>
        /// <param name="action"></param>
        public void RequestShuffleState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                _api.Player_SetShuffle(!_api.Player_GetShuffle());
            }

            _api.Player_GetShuffle();
        }

        /// <summary>
        ///     Changes the player mute state. If the StateAction is Toggle then the current state is switched with it's opposite,
        ///     if it is State the current state is dispatched with an Event.
        /// </summary>
        /// <param name="action"></param>
        public void RequestMuteState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                _api.Player_SetMute(!_api.Player_GetMute());
            }

            _api.Player_GetMute();
        }

        /// <summary>
        /// </summary>
        /// <param name="action"></param>
        public void RequestScrobblerState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                _api.Player_SetScrobbleEnabled(!_api.Player_GetScrobbleEnabled());
            }

            _api.Player_GetScrobbleEnabled();
        }

        /// <summary>
        ///     If the action equals toggle then it changes the repeat state, in any other case
        ///     it just returns the current value of the repeat.
        /// </summary>
        /// <param name="action">toggle or state</param>
        /// <returns>Repeat state: None, All, One</returns>
        public void RequestRepeatState(StateAction action)
        {
            if (action == StateAction.Toggle)
            {
                switch (_api.Player_GetRepeat())
                {
                    case Plugin.RepeatMode.None:
                        _api.Player_SetRepeat(Plugin.RepeatMode.All);
                        break;
                    case Plugin.RepeatMode.All:
                        _api.Player_SetRepeat(Plugin.RepeatMode.None);
                        break;
                    case Plugin.RepeatMode.One:
                        _api.Player_SetRepeat(Plugin.RepeatMode.None);
                        break;
                }
            }
            _api.Player_GetRepeat();
        }


        /// <summary>
        ///     Requests the Now Playing index lyrics. If the lyrics are available then they are dispatched along with
        ///     and event. If not, and the ApiRevision is equal or greater than r17 a request for the downloaded lyrics
        ///     is initiated. The lyrics are dispatched along with and event when ready.
        /// </summary>
        public void RequestNowPlayingTrackLyrics()
        {
            if (!String.IsNullOrEmpty(_api.NowPlaying_GetLyrics()))
            {
                //SendSocketMessage(Constants.NowPlayingLyrics, Constants.Reply, _api.NowPlaying_GetLyrics());
            }
            else if (_api.ApiRevision >= 17)
            {
                var lyrics = _api.NowPlaying_GetDownloadedLyrics();
                //SendSocketMessage(Constants.NowPlayingLyrics, Constants.Reply, 
//                    !String.IsNullOrEmpty(lyrics) ?
//                        lyrics :
//                        "Retrieving Lyrics");
            }
        }


        /// <summary>
        ///     This function requests or changes the AutoDJ functionality's state.
        /// </summary>
        /// <param name="action">
        ///     The action can be either toggle or state.
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
            //SendSocketMessage(Constants.PlayerAutoDj, Constants.Reply, _api.Player_GetAutoDjEnabled());
        }


        /// <summary>
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
        }
    }
}