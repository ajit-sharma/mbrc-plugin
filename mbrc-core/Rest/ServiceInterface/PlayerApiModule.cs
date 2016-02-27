namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.Modules;
    using MusicBeeRemoteCore.Rest.ServiceModel;
    using MusicBeeRemoteCore.Rest.ServiceModel.Enum;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    /// <summary>
    /// The player API module provides the endpoints related to the player functionality.
    /// </summary>
    public class PlayerApiModule : NancyModule
    {
        /// <summary>
        /// The module provides the player API related functionality
        /// </summary>
        private readonly PlayerModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        public PlayerApiModule(PlayerModule module)
            : base("/player")
        {
            this.module = module;

            this.Get["/shuffle"] = _ =>
                {
                    var response = new ShuffleResponse { State = this.module.GetShuffleState() };
                    return this.Response.AsJson(response);
                };

            this.Put["/shuffle"] = _ =>
                {
                    var request = this.Bind<SetShuffleState>();
                    var success = request.Status != null
                                      ? this.module.SetShuffleState((Shuffle)request.Status)
                                      : this.module.ToggleShuffleState();

                    var code = success ? ApiCodes.Success : ApiCodes.Failure;
                    var shuffleState = new ShuffleState { Code = code, State = this.module.GetShuffleState() };
                    return this.Response.AsJson(shuffleState);
                };

            this.Get["/action"] = _ =>
                {
                    var action = (string)this.Request.Query["action"];
                    bool success;
                    switch (action)
                    {
                        case "next":
                            success = this.module.PlayNextTrack();
                            break;
                        case "pause":
                            success = this.module.PausePlayback();
                            break;
                        case "play":
                            success = this.module.StartPlayback();
                            break;
                        case "playpause":
                            success = this.module.PlayPause();
                            break;
                        case "previous":
                            success = this.module.PlayPreviousTrack();
                            break;
                        case "stop":
                            success = this.module.StopPlayback();
                            break;
                        default:
                            success = false;
                            break;
                    }

                    var response = new ResponseBase { Code = success ? ApiCodes.Success : ApiCodes.Failure };
                    return this.Response.AsJson(response);
                };

            this.Get["/status"] = _ => this.Response.AsJson(this.module.GetPlayerStatus());

            this.Get["/volume"] = _ =>
                {
                    var response = new VolumeResponse { Value = this.module.GetVolume() };
                    return this.Response.AsJson(response);
                };

            this.Put["/volume"] = _ =>
                {
                    var request = this.Bind<SetVolume>();
                    var code = this.module.SetVolume(request.Value) ? ApiCodes.Success : ApiCodes.Failure;

                    var response = new VolumeResponse() { Code = code, Value = this.module.GetVolume() };

                    return this.Response.AsJson(response);
                };

            this.Get["/scrobble"] = _ =>
                {
                    var response = new StatusResponse { Enabled = this.module.GetScrobbleState() };
                    return this.Response.AsJson(response);
                };

            this.Put["/scrobble"] = _ =>
                {
                    var request = this.Bind<SetScrobbleStatus>();
                    var success = request.Enabled != null
                                      ? this.module.SetScrobbleState((bool)request.Enabled)
                                      : this.module.SetScrobbleState(!this.module.GetScrobbleState());

                    var response = new StatusResponse
                                       {
                                           Code = success ? ApiCodes.Success : ApiCodes.Failure, 
                                           Enabled = this.module.GetScrobbleState()
                                       };

                    return this.Response.AsJson(response);
                };

            this.Get["/mute"] = _ =>
                {
                    var response = new StatusResponse { Enabled = this.module.GetMuteState() };
                    return this.Response.AsJson(response);
                };

            this.Put["/mute"] = _ =>
                {
                    var request = this.Bind<SetMuteStatus>();
                    var success = request.Enabled != null
                                      ? this.module.SetMuteState((bool)request.Enabled)
                                      : this.module.SetMuteState(!this.module.GetMuteState());

                    var response = new StatusResponse
                                       {
                                           Code = success ? ApiCodes.Success : ApiCodes.Failure, 
                                           Enabled = this.module.GetMuteState()
                                       };
                    return this.Response.AsJson(response);
                };

            this.Get["/repeat"] = _ =>
                {
                    var response = new ValueResponse { Value = this.module.GetRepeatState() };
                    return this.Response.AsJson(response);
                };

            this.Put["/repeat"] = _ =>
                {
                    var request = this.Bind<SetRepeatMode>();
                    var success = request.Mode != null
                                      ? this.module.SetRepeatState((ApiRepeatMode)request.Mode)
                                      : this.module.ChangeRepeatMode();

                    var response = new ValueResponse()
                                       {
                                           Code = success ? ApiCodes.Success : ApiCodes.Failure, 
                                           Value = this.module.GetRepeatState()
                                       };
                    return this.Response.AsJson(response);
                };

            this.Get["/playstate"] = _ =>
                {
                    var response = new ValueResponse { Value = this.module.GetPlayState() };
                    return this.Response.AsJson(response);
                };

            this.Get["/output"] = _ =>
                {
                    var outputDevices = this.module.GetOutputDevices();
                    var response = new OutputDeviceResponse
                                       {
                                           Active = outputDevices.Active, 
                                           Devices = outputDevices.Devices, 
                                           Code = ApiCodes.Success
                                       };
                    return this.Response.AsJson(response);
                };

            this.Put["/repeat"] = _ =>
                {
                    var request = this.Bind<PutOutputDevice>();
                    var success = this.module.SetOutputDevice(request.Active);
                    var outputDevices = this.module.GetOutputDevices();
                    var response = new OutputDeviceResponse
                                       {
                                           Active = outputDevices.Active, 
                                           Devices = outputDevices.Devices, 
                                           Code = success ? ApiCodes.Success : ApiCodes.Failure
                                       };

                    return this.Response.AsJson(response);
                };
        }
    }
}