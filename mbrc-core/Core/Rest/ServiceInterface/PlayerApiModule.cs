using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Rest.ServiceModel;
using MusicBeeRemote.Core.Rest.ServiceModel.Enum;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using Nancy;
using Nancy.ModelBinding;
using ShuffleState = MusicBeeRemote.Core.Enumerations.ShuffleState;

namespace MusicBeeRemote.Core.Rest.ServiceInterface
{
    /// <summary>
    /// The player API module provides the endpoints related to the player functionality.
    /// </summary>
    public class PlayerApiModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerApiModule"/> class.
        /// </summary>
        /// <param name="adapter">
        /// The module.
        /// </param>
        public PlayerApiModule(IPlayerApiAdapter adapter) : base("/player")
        {
            Get["/shuffle"] = _ =>
            {
                var response = new ShuffleResponse
                {
                    State = adapter.GetShuffleState()
                };
                return Response.AsJson(response);
            };

            Put["/shuffle"] = _ =>
            {
                var previousState = adapter.GetShuffleState();
                var newShuffleState = adapter.SwitchShuffle();
                var success = previousState != newShuffleState;

                var code = success ? ApiCodes.Success : ApiCodes.Failure;
                var shuffleState = new ShuffleStateResponse
                {
                    Code = code,
                    State = adapter.GetShuffleState()
                };
                return Response.AsJson(shuffleState);
            };

            Get["/action"] = _ =>
            {
                var action = (string) Request.Query["action"];
                bool success;
                switch (action)
                {
                    case "next":
                        success = adapter.PlayNext();
                        break;
                    case "pause":
                        success = adapter.Pause();
                        break;
                    case "play":
                        success = adapter.Play();
                        break;
                    case "playpause":
                        success = adapter.PlayPause();
                        break;
                    case "previous":
                        success = adapter.PlayPrevious();
                        break;
                    case "stop":
                        success = adapter.StopPlayback();
                        break;
                    default:
                        success = false;
                        break;
                }

                var response = new ResponseBase
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure
                };
                return Response.AsJson(response);
            };

            Get["/status"] = _ => Response.AsJson(adapter.GetStatus());

            Get["/volume"] = _ =>
            {
                var response = new VolumeResponse
                {
                    Value = adapter.GetVolume()
                };
                return Response.AsJson(response);
            };

            Put["/volume"] = _ =>
            {
                var request = this.Bind<SetVolume>();
                var code = adapter.SetVolume(request.Value) ? ApiCodes.Success : ApiCodes.Failure;

                var response = new VolumeResponse
                {
                    Code = code,
                    Value = adapter.GetVolume()
                };

                return Response.AsJson(response);
            };

            Get["/scrobble"] = _ =>
            {
                var response = new StatusResponse
                {
                    Enabled = adapter.ScrobblingEnabled()
                };
                return Response.AsJson(response);
            };

            Put["/scrobble"] = _ =>
            {
                var success = adapter.ToggleScrobbling();
                var response = new StatusResponse
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure,
                    Enabled = adapter.ScrobblingEnabled()
                };

                return Response.AsJson(response);
            };

            Get["/mute"] = _ =>
            {
                var response = new StatusResponse
                {
                    Enabled = adapter.IsMuted()
                };
                return Response.AsJson(response);
            };

            Put["/mute"] = _ =>
            {
                var success = adapter.ToggleMute();

                var response = new StatusResponse
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure,
                    Enabled = adapter.IsMuted()
                };
                return Response.AsJson(response);
            };

            Get["/repeat"] = _ =>
            {
                var response = new ValueResponse
                {
                    Value = adapter.GetRepeatMode().ToString()
                };
                return Response.AsJson(response);
            };

            Put["/repeat"] = _ =>
            {            
                var success = adapter.ToggleRepeatMode();

                var response = new ValueResponse
                {
                    Code = success ? ApiCodes.Success : ApiCodes.Failure,
                    Value = adapter.GetRepeatMode().ToString()
                };
                return Response.AsJson(response);
            };

            Get["/playstate"] = _ =>
            {
                var response = new ValueResponse {Value = adapter.GetState().ToString()};
                return Response.AsJson(response);
            };

            //todo: move to OutputApi
//            Get["/output"] = _ =>
//                {
//                    var outputDevices = adapter.GetOutputDevices();
//                    var response = new OutputDeviceResponse
//                                       {
//                                           Active = outputDevices.ActiveDeviceName, 
//                                           Devices = outputDevices.DeviceNames, 
//                                           Code = ApiCodes.Success
//                                       };
//                    return Response.AsJson(response);
//                };
//
//            Put["/output"] = _ =>
//                {
//                    var request = this.Bind<PutOutputDevice>();
//                    var success = adapter.SetOutputDevice(request.Active);
//                    var outputDevices = adapter.GetOutputDevices();
//                    var response = new OutputDeviceResponse
//                                       {
//                                           Active = outputDevices.ActiveDeviceName, 
//                                           Devices = outputDevices.DeviceNames, 
//                                           Code = success ? ApiCodes.Success : ApiCodes.Failure
//                                       };
//
//                    return Response.AsJson(response);
//                };
        }
    }
}