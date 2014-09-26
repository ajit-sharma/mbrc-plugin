#region

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class PlayerService : Service
    {
        private readonly PlayerModule _module;

        public PlayerService(PlayerModule module)
        {
            _module = module;
        }

        public StatusResponse Get(GetShuffleState request)
        {
            return new StatusResponse
            {
                Enabled = _module.GetShuffleState()
            };
        }

        public SuccessResponse Put(SetShuffleState request)
        {
            return new SuccessResponse
            {
                success = _module.SetShuffleState(request.enabled)
            };
        }

        public SuccessResponse Get(PlaybackPause request)
        {
            return new SuccessResponse
            {
                success = _module.PausePlayback()
            };
        }

        public SuccessResponse Get(PlaybackStart request)
        {
            return new SuccessResponse
            {
                success = _module.StartPlayback()
            };
        }

        public SuccessResponse Get(PlaybackStop request)
        {
            return new SuccessResponse
            {
                success = _module.StopPlayback()
            };
        }

        public SuccessResponse Get(PlayPrevious request)
        {
            return new SuccessResponse
            {
                success = _module.PlayPreviousTrack()
            };
        }

        public SuccessResponse Get(PlayNext request)
        {
            return new SuccessResponse
            {
                success = _module.PlayNextTrack()
            };
        }

        public PlayerStatus Get(GetPlayerStatus request)
        {
            return _module.GetPlayerStatus();
        }

        public StatusResponse Get(GetAutoDjStatus request)
        {
            return new StatusResponse
            {
                Enabled = _module.GetAutoDjState()
            };
        }

        public SuccessResponse Put(SetAutoDjStatus request)
        {
            return new SuccessResponse
            {
                success = _module.SetAutoDjState(request.enabled)
            };
        }

        public VolumeResponse Get(GetVolume request)
        {
            return new VolumeResponse
            {
                Value = _module.GetVolume()
            };
        }

        public SuccessResponse Put(SetVolume request)
        {
            return new SuccessResponse
            {
                success = _module.SetVolume(request.value)
            };
        }

        public StatusResponse Get(GetScrobbleStatus request)
        {
            return new StatusResponse
            {
                Enabled = _module.GetScrobbleState()
            };
        }

        public SuccessResponse Put(SetScrobbleStatus request)
        {
            return new SuccessResponse
            {
                success = _module.SetScrobbleState(request.enabled)
            };
        }

        public StatusResponse Get(GetMuteStatus request)
        {
            return new StatusResponse
            {
                Enabled = _module.GetMuteState()
            };
        }

        public SuccessResponse Put(SetMuteStatus request)
        {
            return new SuccessResponse
            {
                success = _module.SetMuteState(request.enabled)
            };
        }

        public ValueResponse Get(GetRepeatMode request)
        {
            return new ValueResponse
            {
                Value = _module.GetRepeatState()
            };
        }

        public SuccessResponse Put(SetRepeatMode request)
        {
            return new SuccessResponse
            {
                success = _module.SetRepeatState(request.mode)
            };
        }

        public ValueResponse Get(GetPlayState request)
        {
            return new ValueResponse
            {
                Value = _module.GetPlayState()
            };
        }
    }
}