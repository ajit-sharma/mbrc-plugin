using System;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Feature.Player;
using MusicBeeRemote.Core.Network.Http.Responses.Type;

namespace MusicBeeRemoteTester.Adapters
{
    internal class PlayerApiAdapter : IPlayerApiAdapter
    {
        public ShuffleState GetShuffleState()
        {
            throw new NotImplementedException();
        }

        public Repeat GetRepeatMode()
        {
            throw new NotImplementedException();
        }

        public bool ToggleRepeatMode()
        {
            throw new NotImplementedException();
        }

        public bool ScrobblingEnabled()
        {
            throw new NotImplementedException();
        }

        public bool PlayNext()
        {
            throw new NotImplementedException();
        }

        public bool PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public bool StopPlayback()
        {
            throw new NotImplementedException();
        }

        public bool PlayPause()
        {
            throw new NotImplementedException();
        }

        public bool Play()
        {
            throw new NotImplementedException();
        }

        public bool Pause()
        {
            throw new NotImplementedException();
        }

        public PlayerStatus GetStatus()
        {
            throw new NotImplementedException();
        }

        public PlayerState GetState()
        {
            throw new NotImplementedException();
        }

        public bool ToggleScrobbling()
        {
            throw new NotImplementedException();
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public bool SetVolume(int volume)
        {
            throw new NotImplementedException();
        }

        public ShuffleState SwitchShuffle()
        {
            throw new NotImplementedException();
        }

        public bool IsMuted()
        {
            throw new NotImplementedException();
        }

        public bool ToggleMute()
        {
            throw new NotImplementedException();
        }
    }
}