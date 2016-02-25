namespace MusicBeeRemoteCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.Model;
    using MusicBeeRemoteCore.Rest.ServiceInterface;
    using MusicBeeRemoteCore.Rest.ServiceModel.Enum;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteCore.AndroidRemote.Enumerations;

    class PlayerApiAdapter : IPlayerApiAdapter
    {
        public Plugin.MusicBeeApiInterface api;
        
        public PlayerApiAdapter(Plugin.MusicBeeApiInterface api)
        {
            this.api = api;
        }

        public bool ChangeAutoDj(bool enabled)
        {
            return enabled ? this.api.Player_StartAutoDj() : this.api.Player_EndAutoDj();
        }

        public bool ChangeRepeat()
        {
            var repeat = this.api.Player_GetRepeat();
            Plugin.RepeatMode newMode;
            switch (repeat)
            {
                case Plugin.RepeatMode.None:
                    newMode = Plugin.RepeatMode.All;
                    break;
                case Plugin.RepeatMode.All:
                    newMode = Plugin.RepeatMode.One;
                    break;
                default:
                    newMode = Plugin.RepeatMode.None;
                    break;
            }

            return this.api.Player_SetRepeat(newMode);
        }
    
        public bool GetAutoDjState()
        {
            return this.api.Player_GetAutoDjEnabled();
        }
    
        public bool GetMuteState()
        {
            return this.api.Player_GetMute();
        }

        public OutputDevice GetOutputDevices()
        {
            string[] devices;
            string active;
            this.api.Player_GetOutputDevices(out devices, out active);

            return new OutputDevice { Active = active, Devices = devices };
        }

        public string GetPlayState()
        {
            return this.api.Player_GetPlayState().ToString().ToLower();
        }

        public string GetRepeatState()
        {
            var repeatState = this.api.Player_GetRepeat().ToString();
            return repeatState.ToLower();
        }

        public bool GetScrobbleState()
        {
            return this.api.Player_GetScrobbleEnabled();
        }

        public Shuffle GetShuffleState()
        {
            Shuffle state;
            if (this.api.Player_GetAutoDjEnabled())
            {
                state = Shuffle.autodj;
            }
            else
            {
                var shuffleEnabled = this.api.Player_GetShuffle();
                state = shuffleEnabled ? Shuffle.shuffle : Shuffle.off;
            }

            return state;
        }

        public PlayerStatus GetStatus()
        {
            return new PlayerStatus
                       {
                           Repeat = this.api.Player_GetRepeat().ToString().ToLower(), 
                           Mute = this.api.Player_GetMute(), 
                           Shuffle = this.GetShuffleState(), 
                           Scrobble = this.api.Player_GetScrobbleEnabled(), 
                           PlayerState = this.api.Player_GetPlayState().ToString().ToLower(), 
                           Volume = (int)Math.Round(this.api.Player_GetVolume() * 100, 1), 
                           Code = ApiCodes.Success
                       };
        }
        
        public int GetVolume()
        {
            return (int)Math.Round(this.api.Player_GetVolume() * 100, 1);
        }

        public bool PausePlayback()
        {
            var success = false;
            var playState = this.api.Player_GetPlayState();
            if (playState == Plugin.PlayState.Playing)
            {
                success = this.api.Player_PlayPause();
            }

            return success;
        }

        public bool PlayNext()
        {
            return this.api.Player_PlayNextTrack();
        }

        public bool PlayPause()
        {
            return this.api.Player_PlayPause();
        }

        public bool PlayPrevious()
        {
            return this.api.Player_PlayPreviousTrack();
        }

        public bool SetMute(bool enabled)
        {
            return this.api.Player_SetMute(enabled);
        }

        public bool SetOutputDevice(string active)
        {
            return this.api.Player_SetOutputDevice(active);
        }

        public bool SetRepeatState(ApiRepeatMode mode)
        {
            var success = false;
            Plugin.RepeatMode repeatMode;

            switch (mode)
            {
                case ApiRepeatMode.all:
                    repeatMode = Plugin.RepeatMode.All;
                    break;
                case ApiRepeatMode.none:
                    repeatMode = Plugin.RepeatMode.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            success = this.api.Player_SetRepeat(repeatMode);

            return success;
        }

        public bool SetScrobbleState(bool enabled)
        {
            return this.api.Player_SetScrobbleEnabled(enabled);
        }

        public bool SetShuffleState(Shuffle state)
        {
            var success = false;
            switch (state)
            {
                case Shuffle.autodj:
                    success = this.api.Player_StartAutoDj();
                    break;
                case Shuffle.off:
                    success = this.api.Player_SetShuffle(false);
                    break;
                case Shuffle.shuffle:
                    success = this.api.Player_SetShuffle(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            return success;
        }

        public bool SetVolume(int volume)
        {
            var success = false;
            if (volume >= 0)
            {
                success = this.api.Player_SetVolume((float)volume / 100);
            }

            if (this.api.Player_GetMute())
            {
                success = this.api.Player_SetMute(false);
            }

            return success;
        }

        public bool StartPlayback()
        {
            var success = false;
            var playState = this.api.Player_GetPlayState();
            if (playState != Plugin.PlayState.Playing)
            {
                success = this.api.Player_PlayPause();
            }

            return success;
        }

        public bool StopPlayback()
        {
            return this.api.Player_Stop();
        }

        public bool ToggleShuffle()
        {
            var success = false;
            var shuffleEnabled = this.api.Player_GetShuffle();
            var autoDjEnabled = this.api.Player_GetAutoDjEnabled();

            if (shuffleEnabled && !autoDjEnabled)
            {
                success = this.api.Player_StartAutoDj();
            }
            else if (autoDjEnabled)
            {
                success = this.api.Player_EndAutoDj();
            }
            else
            {
                success = this.api.Player_SetShuffle(true);
            }

            return success;
        }
    }
}