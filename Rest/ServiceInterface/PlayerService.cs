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
            return new SuccessStatusResponse
            {
                Success = _module.SetShuffleState(request.enabled),
                Enabled = _module.GetShuffleState()
            };
        }

        public SuccessStatusResponse Put(ToggleShuffleState request)
        {
            return new SuccessStatusResponse
            {
                Success = _module.SetShuffleState(!_module.GetShuffleState()),
                Enabled = _module.GetShuffleState()
            };
        }

        public SuccessResponse Get(PlaybackPause request)
        {
            return new SuccessResponse
            {
                Success = _module.PausePlayback()
            };
        }

        public SuccessResponse Get(PlaybackStart request)
        {
            return new SuccessResponse
            {
                Success = _module.StartPlayback()
            };
        }

        public SuccessResponse Get(PlaybackStop request)
        {
            return new SuccessResponse
            {
                Success = _module.StopPlayback()
            };
        }

        public SuccessResponse Get(PlayPrevious request)
        {
            return new SuccessResponse
            {
                Success = _module.PlayPreviousTrack()
            };
        }

        public SuccessResponse Get(PlayNext request)
        {
            return new SuccessResponse
            {
                Success = _module.PlayNextTrack()
            };
        }

        public PlayerStatus Get(GetPlayerStatus request)
        {
            return _module.GetPlayerStatus();
        }

        public SuccessResponse Put(PlaybackPlayPause request)
        {
            return new SuccessResponse
            {
                Success = _module.PlayPause()
            };
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
            return new SuccessStatusResponse
            {
                Success = _module.SetAutoDjState(request.enabled),
                Enabled = _module.GetAutoDjState()
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
            return new SuccessVolumeResponse()
            {
                Success = _module.SetVolume(request.value),
                Value = _module.GetVolume()
            };
        }

        public StatusResponse Get(GetScrobbleStatus request)
        {
            return new StatusResponse
            {
                Enabled = _module.GetScrobbleState()
            };
        }

        public SuccessStatusResponse Put(SetScrobbleStatus request)
        {
            return new SuccessStatusResponse
            {
                Success = _module.SetScrobbleState(request.enabled),
                Enabled = _module.GetScrobbleState()
                
            };
        }

        public SuccessStatusResponse Put(ToggleScrobbleStatus request)
        {
            return new SuccessStatusResponse
            {
                Success = _module.SetScrobbleState(!_module.GetScrobbleState()),
                Enabled = _module.GetScrobbleState()
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
            return new SuccessStatusResponse
            {
                Success = _module.SetMuteState(request.enabled),
                Enabled = _module.GetMuteState()
            };
        }

        public SuccessStatusResponse Put(ToggleMuteStatus request)
        {
            return new SuccessStatusResponse
            {
                Success = _module.SetMuteState(!_module.GetMuteState()),
                Enabled = _module.GetMuteState()
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
            return new SuccessValueResponse
            {
                Success = _module.SetRepeatState(request.mode),
                Value = _module.GetRepeatState()
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