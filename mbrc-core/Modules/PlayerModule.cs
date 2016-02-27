namespace MusicBeeRemoteCore.Modules
{
    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.Rest.ServiceModel.Enum;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    public class PlayerModule
    {
        private readonly IPlayerApiAdapter api;

        public PlayerModule(IPlayerApiAdapter api)
        {
            this.api = api;
        }

        public bool ChangeRepeatMode()
        {
            return this.api.ChangeRepeat();
        }

        public bool GetAutoDjState()
        {
            return this.api.GetAutoDjState();
        }

        public bool GetMuteState()
        {
            return this.api.GetMuteState();
        }

        public OutputDevice GetOutputDevices()
        {
            return this.api.GetOutputDevices();
        }

        public PlayerStatus GetPlayerStatus()
        {
            return this.api.GetStatus();
        }

        public string GetPlayState()
        {
            return this.api.GetPlayState();
        }

        public string GetRepeatState()
        {
            return this.api.GetRepeatState();
        }

        public bool GetScrobbleState()
        {
            return this.api.GetScrobbleState();
        }

        public Shuffle GetShuffleState()
        {
            return this.api.GetShuffleState();
        }

        public int GetVolume()
        {
            return this.api.GetVolume();
        }

        public bool PausePlayback()
        {
            return this.api.PausePlayback();
        }

        public bool PlayNextTrack()
        {
            return this.api.PlayNext();
        }

        public bool PlayPause()
        {
            return this.api.PlayPause();
        }

        public bool PlayPreviousTrack()
        {
            return this.api.PlayPrevious();
        }

        public bool SetAutoDjState(bool enabled)
        {
            return this.api.ChangeAutoDj(enabled);
        }

        public bool SetMuteState(bool enabled)
        {
            return this.api.SetMute(enabled);
        }

        public bool SetOutputDevice(string active)
        {
            return this.api.SetOutputDevice(active);
        }

        public bool SetRepeatState(ApiRepeatMode mode)
        {
            return this.api.SetRepeatState(mode);
        }

        public bool SetScrobbleState(bool enabled)
        {
            return this.api.SetScrobbleState(enabled);
        }

        public bool SetShuffleState(Shuffle state)
        {
            return this.api.SetShuffleState(state);
        }

        public bool SetVolume(int volume)
        {
            return this.api.SetVolume(volume);
        }

        public bool StartPlayback()
        {
            return this.api.StartPlayback();
        }

        public bool StopPlayback()
        {
            return this.api.StopPlayback();
        }

        public bool ToggleShuffleState()
        {
            return this.api.ToggleShuffle();
        }
    }
}