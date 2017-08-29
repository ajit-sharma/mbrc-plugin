using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Enumerations;
using MusicBeeRemote.Core.Rest.ServiceModel.Enum;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;

namespace plugin_tester
{
    using System;

    internal class PlayerApiAdapter : IPlayerApiAdapter
    {
        public bool ChangeAutoDj(bool enabled)
        {
            throw new NotImplementedException();
        }

        public bool ChangeRepeat()
        {
            throw new NotImplementedException();
        }

        public bool GetAutoDjState()
        {
            throw new NotImplementedException();
        }

        public bool GetMuteState()
        {
            throw new NotImplementedException();
        }

        public OutputDevice GetOutputDevices()
        {
            throw new NotImplementedException();
        }

        public string GetPlayState()
        {
            throw new NotImplementedException();
        }

        public string GetRepeatState()
        {
            throw new NotImplementedException();
        }

        public bool GetScrobbleState()
        {
            throw new NotImplementedException();
        }

        public ShuffleState GetShuffleState()
        {
            throw new NotImplementedException();
        }

        public PlayerStatus GetStatus()
        {
            throw new NotImplementedException();
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public bool PausePlayback()
        {
            throw new NotImplementedException();
        }

        public bool PlayNext()
        {
            throw new NotImplementedException();
        }

        public bool PlayPause()
        {
            throw new NotImplementedException();
        }

        public bool PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public bool SetMute(bool enabled)
        {
            throw new NotImplementedException();
        }

        public bool SetOutputDevice(string active)
        {
            throw new NotImplementedException();
        }

        public bool SetRepeatState(ApiRepeatMode mode)
        {
            throw new NotImplementedException();
        }

        public bool SetScrobbleState(bool enabled)
        {
            throw new NotImplementedException();
        }

        public bool SetShuffleState(ShuffleState state)
        {
            throw new NotImplementedException();
        }

        public bool SetVolume(int volume)
        {
            throw new NotImplementedException();
        }

        public bool StartPlayback()
        {
            throw new NotImplementedException();
        }

        public bool StopPlayback()
        {
            throw new NotImplementedException();
        }

        public bool ToggleShuffle()
        {
            throw new NotImplementedException();
        }
    }
}