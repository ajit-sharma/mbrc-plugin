#region

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Enum;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Nancy;
using Nancy.ModelBinding;
using ShuffleState = MusicBeePlugin.Rest.ServiceModel.ShuffleState;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class PlayerApiModule : NancyModule
    {
        private readonly PlayerModule _module;

        public PlayerApiModule(PlayerModule module)
        {
            _module = module;

            Get["/player/shuffle"] = _ => new ShuffleResponse
            {
                State = _module.GetShuffleState()
            };

            Put["/player/shuffle"] = _ =>
            {
                var request = this.Bind<SetShuffleState>();
                var success = request.Status != null
                    ? _module.SetShuffleState((AndroidRemote.Enumerations.ShuffleState) request.Status)
                    : _module.ToggleShuffleState();

                return new ShuffleState
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure,
                    State = _module.GetShuffleState()
                };
            };

            Get["/player/action"] = _ =>
            {
                var action = (string) Request.Query["action"];
                bool success;
                switch (action)
                {
                    case "next":
                        success = _module.PlayNextTrack();
                        break;
                    case "pause":
                        success = _module.PausePlayback();
                        break;
                    case "play":
                        success = _module.StartPlayback();
                        break;
                    case "playpause":
                        success = _module.PlayPause();
                        break;
                    case "previous":
                        success = _module.PlayPreviousTrack();
                        break;
                    case "stop":
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
            };

            Get["/player/status"] = _ => _module.GetPlayerStatus();

            Get["/player/volume"] = _ => new VolumeResponse
            {
                Value = _module.GetVolume()
            };

            Put["/player/volume"] = _ =>
            {
                var request = this.Bind<SetVolume>();
                return new VolumeResponse()
                {
                    Code = _module.SetVolume(request.Value) ? ApiCodes.Success : ApiCodes.Failure,
                    Value = _module.GetVolume()
                };
            };

            Get["/player/scrobble"] = _ => new StatusResponse
            {
                Enabled = _module.GetScrobbleState()
            };

            Put["/player/scrobble"] = _ =>
            {
                var request = this.Bind<SetScrobbleStatus>();
                var success = request.Enabled != null
                    ? _module.SetScrobbleState((bool) request.Enabled)
                    : _module.SetScrobbleState(!_module.GetScrobbleState());

                return new StatusResponse
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure,
                    Enabled = _module.GetScrobbleState()
                };
            };

            Get["/player/mute"] = _ => new StatusResponse
            {
                Enabled = _module.GetMuteState()
            };

            Put["/player/mute"] = _ =>
            {
                var request = this.Bind<SetMuteStatus>();
                var success = request.Enabled != null
                    ? _module.SetMuteState((bool) request.Enabled)
                    : _module.SetMuteState(!_module.GetMuteState());
                return new StatusResponse
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure,
                    Enabled = _module.GetMuteState()
                };
            };

            Get["/player/repeat"] = _ => new ValueResponse
            {
                Value = _module.GetRepeatState()
            };

            Put["/player/repeat"] = _ =>
            {
                var request = this.Bind<SetRepeatMode>();
                var success = request.Mode != null
                    ? _module.SetRepeatState((ApiRepeatMode) request.Mode)
                    : _module.ChangeRepeatMode();

                return new ValueResponse()
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure,
                    Value = _module.GetRepeatState()
                };
            };

            Get["/player/playstate"] = _ => new ValueResponse
            {
                Value = _module.GetPlayState()
            };

            Get["/player/output"] = _ =>
            {
                var outputDevices = _module.GetOutputDevices();
                return new OutputDeviceResponse
                {
                    Active = outputDevices.Active,
                    Devices = outputDevices.Devices,
                    Code = ApiCodes.Success
                };
            };

            Put["/player/repeat"] = _ =>
            {
                var request = this.Bind<PutOutputDevice>();
                var success = _module.SetOutputDevice(request.Active);
                var outputDevices = _module.GetOutputDevices();
                return new OutputDeviceResponse
                {
                    Active = outputDevices.Active,
                    Devices = outputDevices.Devices,
                    Code = success ? ApiCodes.Success : ApiCodes.Failure
                };
            };
        }
    }
}