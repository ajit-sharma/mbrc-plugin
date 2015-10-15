#region

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Enum;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;
using ShuffleState = MusicBeePlugin.Rest.ServiceModel.ShuffleState;

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

        public ShuffleResponse Get(GetShuffleState request)
        {
            return new ShuffleResponse
            {
                State = _module.GetShuffleState()
            };
        }

        public ResponseBase Put(SetShuffleState request)
        {
            
            var success = request.Status != null
                ? _module.SetShuffleState((AndroidRemote.Enumerations.ShuffleState) request.Status)
                : _module.ToggleShuffleState();

            return new ShuffleState
            {
                Code = success ? ApiCodes.Success : ApiCodes.Failure,
                State = _module.GetShuffleState()
            };
        }
		
        public ResponseBase Get(PlayerAction request)
        {
	        bool success;
	        switch (request.Action)
	        {
				case PlaybackAction.next:
			        success = _module.PlayNextTrack();
			        break;
				case PlaybackAction.pause:
			        success = _module.PausePlayback();
			        break;
				case PlaybackAction.play:
			        success = _module.StartPlayback();
			        break;
				case PlaybackAction.playpause:
			        success = _module.PlayPause();
			        break;
				case PlaybackAction.previous:
			        success = _module.PlayPreviousTrack();
			        break;
				case PlaybackAction.stop:
			        success = _module.StopPlayback();
			        break;
				default:
			        success = false;
			        break;
	        }
			
	        return new ResponseBase
            {
                Code = success ? ApiCodes.Success : ApiCodes.Failure
            };
        }

        public PlayerStatus Get(GetPlayerStatus request)
        {
            return _module.GetPlayerStatus();
        }
        
        public VolumeResponse Get(GetVolume request)
        {
            return new VolumeResponse
            {
                Value = _module.GetVolume()
            };
        }

        public VolumeResponse Put(SetVolume request)
        {
            return new VolumeResponse()
            {
                Code = _module.SetVolume(request.Value) ? ApiCodes.Success : ApiCodes.Failure,
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

        public StatusResponse Put(SetScrobbleStatus request)
        {
            var success = request.Enabled != null
                ? _module.SetScrobbleState((bool) request.Enabled)
                : _module.SetScrobbleState(!_module.GetScrobbleState());

            return new StatusResponse
            {
                Code = success ? ApiCodes.Success : ApiCodes.Failure,
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

        public StatusResponse Put(SetMuteStatus request)
        {

            var success = request.Enabled != null
                ? _module.SetMuteState((bool) request.Enabled)
                : _module.SetMuteState(!_module.GetMuteState());
            return new StatusResponse
            {
                Code = success ? ApiCodes.Success : ApiCodes.Failure,
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

        public ValueResponse Put(SetRepeatMode request)
        {
            var success = request.Mode != null
                ? _module.SetRepeatState((ApiRepeatMode) request.Mode)
                : _module.ChangeRepeatMode();

            return new ValueResponse()
            {
                Code = success ? ApiCodes.Success : ApiCodes.Failure,
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