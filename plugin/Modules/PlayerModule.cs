#region

using MusicBeePlugin.Rest.ServiceModel.Type;
using System;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Rest.ServiceModel.Enum;
using RepeatMode = MusicBeePlugin.Plugin.RepeatMode;

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

        public bool PlayNextTrack()
        {
            return _api.Player_PlayNextTrack();
        }

        public bool StopPlayback()
        {
            return _api.Player_Stop();
        }

        public bool PlayPreviousTrack()
        {
            return _api.Player_PlayPreviousTrack();
        }

        public int GetVolume()
        {
            return ((int) Math.Round(_api.Player_GetVolume()*100, 1));
        }

        public bool SetVolume(int volume)
        {
            var success = false;
            if (volume >= 0)
            {
                success = _api.Player_SetVolume((float) volume/100);
            }

            if (_api.Player_GetMute())
            {
                success = _api.Player_SetMute(false);
            }
            return success;
        }

        public ShuffleState GetShuffleState()
        {
            ShuffleState state;
            if (_api.Player_GetAutoDjEnabled())
            {
                state = ShuffleState.autodj;
            }
            else
            {
                var shuffleEnabled = _api.Player_GetShuffle();
                state = shuffleEnabled ? ShuffleState.shuffle : ShuffleState.off;
            }
            return state;
        }

        public bool SetShuffleState(ShuffleState state)
        {
            var success = false;
            switch (state)
            {
                case ShuffleState.autodj:
                    success = _api.Player_StartAutoDj();
                    break;
                case ShuffleState.off:
                    success = _api.Player_SetShuffle(false);
                    break;
                case ShuffleState.shuffle:
                    success = _api.Player_SetShuffle(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            return success;
        }

        public bool GetMuteState()
        {
            return _api.Player_GetMute();
        }

        public bool SetMuteState(bool enabled)
        {
            return _api.Player_SetMute(enabled);
        }

        public bool GetScrobbleState()
        {
            return _api.Player_GetScrobbleEnabled();
        }

        public bool SetScrobbleState(bool enabled)
        {
            return _api.Player_SetScrobbleEnabled(enabled);
        }

        public string GetRepeatState()
        {
            var repeatState = _api.Player_GetRepeat().ToString();
            return repeatState.ToLower();
        }

        public bool SetRepeatState(ApiRepeatMode mode)
        {
            var success = false;
            RepeatMode repeatMode;

            switch (mode)
            {
                case ApiRepeatMode.all:
                    repeatMode = RepeatMode.All;
                    break;
                case ApiRepeatMode.none:
                    repeatMode = RepeatMode.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
            success = _api.Player_SetRepeat(repeatMode);

            return success;
        }

        public bool GetAutoDjState()
        {
            return _api.Player_GetAutoDjEnabled();
        }

        public bool SetAutoDjState(bool enabled)
        {
            return enabled ? _api.Player_StartAutoDj() : _api.Player_EndAutoDj();
        }

        public bool PausePlayback()
        {
            var success = false;
            var playState = _api.Player_GetPlayState();
            if (playState == Plugin.PlayState.Playing)
            {
                success = _api.Player_PlayPause();
            }
            return success;
        }

        public bool StartPlayback()
        {
            var success = false;
            var playState = _api.Player_GetPlayState();
            if (playState != Plugin.PlayState.Playing)
            {
                success = _api.Player_PlayPause();
            }
            return success;
        }

        public string GetPlayState()
        {
            return _api.Player_GetPlayState().ToString().ToLower();
        }

        public PlayerStatus GetPlayerStatus()
        {
            return new PlayerStatus
            {
                Repeat = _api.Player_GetRepeat().ToString().ToLower(),
                Mute = _api.Player_GetMute(),
                Shuffle = GetShuffleState(),
                Scrobble = _api.Player_GetScrobbleEnabled(),
                PlayerState = _api.Player_GetPlayState().ToString().ToLower(),
                Volume = ((int) Math.Round(_api.Player_GetVolume()*100, 1))
            };
        }

        public bool PlayPause()
        {
            return _api.Player_PlayPause();
        }

        public bool ChangeRepeatMode()
        {
            var repeat = _api.Player_GetRepeat();
            RepeatMode newMode;
            switch (repeat)
            {
                case RepeatMode.None:
                    newMode = RepeatMode.All;
                    break;
                case RepeatMode.All:
                    newMode = RepeatMode.One;
                    break;
                default:
                    newMode = RepeatMode.None;
                    break;
            }

            return _api.Player_SetRepeat(newMode);
        }

        public bool ToggleShuffleState()
        {
            var success = false;
            var shuffleEnabled = _api.Player_GetShuffle();
            var autoDjEnabled = _api.Player_GetAutoDjEnabled();

            if (shuffleEnabled && !autoDjEnabled)
            {
                success = _api.Player_StartAutoDj();
            }
            else if (autoDjEnabled)
            {
                success = _api.Player_EndAutoDj();
            }
            else
            {
                success = _api.Player_SetShuffle(true);
            }
            return success;
        }
    }
}