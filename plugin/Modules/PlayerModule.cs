#region

using MusicBeePlugin.Rest.ServiceModel.Type;
using System;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Rest.ServiceModel.Enum;

#endregion

namespace MusicBeePlugin.Modules
{
    public class PlayerModule
    {
        private readonly IPlayerApiAdapter api;

        public PlayerModule(IPlayerApiAdapter api)
        {
            this.api = api;
        }

        public bool PlayNextTrack()
        {
            return this.api.PlayNext();
        }

        public bool StopPlayback()
        {
            return this.api.StopPlayback();
        }

        public bool PlayPreviousTrack()
        {
            return this.api.PlayPrevious();
        }

        public int GetVolume()
        {
            return this.api.GetVolume();
        }

        public bool SetVolume(int volume)
        {
            return this.api.SetVolume(volume);
        }
        
        public ShuffleState GetShuffleState()
        {
            return this.api.GetShuffleState();
        }

        public bool SetShuffleState(ShuffleState state)
        {
            return this.api.SetShuffleState(state);
        }

        public bool GetMuteState()
        {
            return this.api.GetMuteState();
        }

        public bool SetMuteState(bool enabled)
        {
            return this.api.SetMute(enabled);
            
        }

        public bool GetScrobbleState()
        {
            return this.api.GetScrobbleState();
        }

        public bool SetScrobbleState(bool enabled)
        {
            return this.api.SetScrobbleState(enabled);
        }

        public string GetRepeatState()
        {
            return this.api.GetRepeatState();
        }

        public bool SetRepeatState(ApiRepeatMode mode)
        {
            return this.api.SetRepeatState(mode);
        }

        public bool GetAutoDjState()
        {
            return this.api.GetAutoDjState();
        }

        public bool SetAutoDjState(bool enabled)
        {
            return this.api.ChangeAutoDj(enabled);
        }

        public bool PausePlayback()
        {
            return this.api.PausePlayback();
        }

        public bool StartPlayback()
        {
            return this.api.StartPlayback();
        }

        public string GetPlayState()
        {
            return this.api.GetPlayState();
        }

        public PlayerStatus GetPlayerStatus()
        {
            return this.api.GetStatus();
        }

        public bool PlayPause()
        {
            return this.api.PlayPause();
        }

        public bool ChangeRepeatMode()
        {
            return this.api.ChangeRepeat();
        }

        public bool ToggleShuffleState()
        {
            return this.api.ToggleShuffle();
        }

        public OutputDevice GetOutputDevices()
        {
            return this.api.GetOutputDevices();
        }

        public bool SetOutputDevice(string active)
        {
            return this.api.SetOutputDevice(active);
        }
    }
}