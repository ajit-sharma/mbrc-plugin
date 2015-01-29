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
            var success = request.Enabled != null 
                ? _module.SetShuffleState((bool) request.Enabled) :
                _module.SetShuffleState(!_module.GetShuffleState());

            return new SuccessStatusResponse
            {
                Success = success,
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
                Success = _module.SetAutoDjState(request.Enabled),
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
                Success = _module.SetVolume(request.Value),
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
            var success = request.Enabled != null
                ? _module.SetScrobbleState((bool) request.Enabled)
                : _module.SetScrobbleState(!_module.GetScrobbleState());

            return new SuccessStatusResponse
            {
                Success = success,
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

            var success = request.Enabled != null
                ? _module.SetMuteState((bool) request.Enabled)
                : _module.SetMuteState(!_module.GetMuteState());
            return new SuccessStatusResponse
            {
                Success = success,
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
            var success = request.Mode != null
                ? _module.SetRepeatState(request.Mode)
                : _module.ChangeRepeatMode();

            return new SuccessValueResponse
            {
                Success = success,
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