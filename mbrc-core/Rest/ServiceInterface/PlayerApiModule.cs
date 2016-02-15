namespace MusicBeePlugin.Rest.ServiceInterface
{
    using MusicBeePlugin.Modules;
    using MusicBeePlugin.Rest.ServiceModel;
    using MusicBeePlugin.Rest.ServiceModel.Enum;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    public class PlayerApiModule : NancyModule
    {
        private readonly PlayerModule _module;

        public PlayerApiModule(PlayerModule module)
        {
            this._module = module;

            this.Get["/player/shuffle"] = _ => new ShuffleResponse { State = this._module.GetShuffleState() };

            this.Put["/player/shuffle"] = _ =>
                {
                    var request = this.Bind<SetShuffleState>();
                    var success = request.Status != null
                                      ? this._module.SetShuffleState(
                                          (AndroidRemote.Enumerations.ShuffleState)request.Status)
                                      : this._module.ToggleShuffleState();

                    return new ShuffleState
                               {
                                   Code = success ? ApiCodes.Success : ApiCodes.Failure, 
                                   State = this._module.GetShuffleState()
                               };
                };

            this.Get["/player/action"] = _ =>
                {
                    var action = (string)this.Request.Query["action"];
                    bool success;
                    switch (action)
                    {
                        case "next":
                            success = this._module.PlayNextTrack();
                            break;
                        case "pause":
                            success = this._module.PausePlayback();
                            break;
                        case "play":
                            success = this._module.StartPlayback();
                            break;
                        case "playpause":
                            success = this._module.PlayPause();
                            break;
                        case "previous":
                            success = this._module.PlayPreviousTrack();
                            break;
                        case "stop":
                            success = this._module.StopPlayback();
                            break;
                        default:
                            success = false;
                            break;
                    }

                    return new ResponseBase { Code = success ? ApiCodes.Success : ApiCodes.Failure };
                };

            this.Get["/player/status"] = _ => this._module.GetPlayerStatus();

            this.Get["/player/volume"] = _ => new VolumeResponse { Value = this._module.GetVolume() };

            this.Put["/player/volume"] = _ =>
                {
                    var request = this.Bind<SetVolume>();
                    return new VolumeResponse()
                               {
                                   Code =
                                       this._module.SetVolume(request.Value)
                                           ? ApiCodes.Success
                                           : ApiCodes.Failure, 
                                   Value = this._module.GetVolume()
                               };
                };

            this.Get["/player/scrobble"] = _ => new StatusResponse { Enabled = this._module.GetScrobbleState() };

            this.Put["/player/scrobble"] = _ =>
                {
                    var request = this.Bind<SetScrobbleStatus>();
                    var success = request.Enabled != null
                                      ? this._module.SetScrobbleState((bool)request.Enabled)
                                      : this._module.SetScrobbleState(!this._module.GetScrobbleState());

                    return new StatusResponse
                               {
                                   Code = success ? ApiCodes.Success : ApiCodes.Failure, 
                                   Enabled = this._module.GetScrobbleState()
                               };
                };

            this.Get["/player/mute"] = _ => new StatusResponse { Enabled = this._module.GetMuteState() };

            this.Put["/player/mute"] = _ =>
                {
                    var request = this.Bind<SetMuteStatus>();
                    var success = request.Enabled != null
                                      ? this._module.SetMuteState((bool)request.Enabled)
                                      : this._module.SetMuteState(!this._module.GetMuteState());
                    return new StatusResponse
                               {
                                   Code = success ? ApiCodes.Success : ApiCodes.Failure, 
                                   Enabled = this._module.GetMuteState()
                               };
                };

            this.Get["/player/repeat"] = _ => new ValueResponse { Value = this._module.GetRepeatState() };

            this.Put["/player/repeat"] = _ =>
                {
                    var request = this.Bind<SetRepeatMode>();
                    var success = request.Mode != null
                                      ? this._module.SetRepeatState((ApiRepeatMode)request.Mode)
                                      : this._module.ChangeRepeatMode();

                    return new ValueResponse()
                               {
                                   Code = success ? ApiCodes.Success : ApiCodes.Failure, 
                                   Value = this._module.GetRepeatState()
                               };
                };

            this.Get["/player/playstate"] = _ => new ValueResponse { Value = this._module.GetPlayState() };

            this.Get["/player/output"] = _ =>
                {
                    var outputDevices = this._module.GetOutputDevices();
                    return new OutputDeviceResponse
                               {
                                   Active = outputDevices.Active, 
                                   Devices = outputDevices.Devices, 
                                   Code = ApiCodes.Success
                               };
                };

            this.Put["/player/repeat"] = _ =>
                {
                    var request = this.Bind<PutOutputDevice>();
                    var success = this._module.SetOutputDevice(request.Active);
                    var outputDevices = this._module.GetOutputDevices();
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