#region

using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    /// <summary>
    ///     Strings representing the HTTP methods (verbs)
    /// </summary>
    public class Verbs
    {
        /// <summary>
        ///     A GET Request.
        /// </summary>
        public const string Get = "GET";

        /// <summary>
        ///     A PUT Request.
        /// </summary>
        public const string Put = "PUT";
    }

    /// <summary>
    ///     The Routes of the API
    /// </summary>
    public class Routes
    {
        public const string PlayerShuffle = "/player/shuffle";
        public const string PlayerScrobble = "/player/scrobble";
        public const string PlayerRepeat = "/player/repeat";
        public const string PlayerMute = "/player/mute";
    }

    /// <summary>
    ///     Short Summary of the API functionality
    /// </summary>
    public class Summary
    {
        public const string ShuffleGet = "Gets the current state of shuffle.";
        public const string ShufflePut = @"Sets the shuffle status.";
    }

    /// <summary>
    ///     Holds descriptions of various parameters
    /// </summary>
    public class Description
    {
        public const string ShuffleEnabled = @"If the value is true or false it will enable/disable shuffle." +
                                             "\n If left empty it will toggle shuffle on/off depending on the previous state.";

        public const string ScrobbleEnabled =
            @"If the value is true or false it will enable/disable last.fm scrobbling." +
            "\n If left empty it will toggle scrobbling on/off depending on the previous state.";

        public const string RepeatMode =
            @"It can be all/none or empty. If left empty it will change between the available states." 
            + @" [Note: As far as I know repeat one is not supported by the MusicBee API.";
    }

    [Route(Routes.PlayerShuffle, Verbs.Get, Summary = Summary.ShuffleGet)]
    public class GetShuffleState : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerShuffle, Verbs.Put, Summary = Summary.ShufflePut)]
    public class SetShuffleState : IReturn<SuccessStatusResponse>
    {
        [ApiMember(Name = "enabled", ParameterType = "query", DataType = "boolean",
            Description = Description.ShuffleEnabled, IsRequired = false)]
        public bool? enabled { get; set; }
    }

    [Route(Routes.PlayerScrobble, "GET", Summary = "Gets the status of last.fm scrobbling")]
    public class GetScrobbleStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerScrobble, "PUT", Summary = "Sets the status of last.fm scrobbling.")]
    public class SetScrobbleStatus : IReturn<SuccessStatusResponse>
    {
        [ApiMember(Name = "enabled", ParameterType = "query", DataType = "boolean",
            Description = Description.ScrobbleEnabled, IsRequired = false)]
        public bool? enabled { get; set; }
    }


    [Route(Routes.PlayerRepeat, "GET")]
    public class GetRepeatMode : IReturn<ValueResponse>
    {
    }

    [Route(Routes.PlayerRepeat, "PUT")]
    public class SetRepeatMode : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "mode", ParameterType = "query", DataType = "string",
            Description = Description.RepeatMode, IsRequired = false)]
        [ApiAllowableValues("mode", "all", "none")]
        public string mode { get; set; }
    }

    [Route(Routes.PlayerMute, "GET")]
    public class GetMuteStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerMute, "PUT")]
    public class SetMuteStatus : IReturn<SuccessStatusResponse>
    {
        public bool enabled { get; set; }
    }

    [Route("/player/mute/toggle", "PUT")]
    public class ToggleMuteStatus : IReturn<SuccessStatusResponse>
    {
    }

    [Route("/player/volume", "GET")]
    public class GetVolume : IReturn<VolumeResponse>
    {
    }

    [Route("/player/volume", "PUT")]
    public class SetVolume : IReturn<SuccessResponse>
    {
        public int value { get; set; }
    }

    [Route("/player/autodj", "GET")]
    public class GetAutoDjStatus : IReturn<StatusResponse>
    {
    }

    [Route("/player/autodj", "PUT")]
    public class SetAutoDjStatus : IReturn<SuccessStatusResponse>
    {
        public bool enabled { get; set; }
    }

    [Route("/player/previous", "GET")]
    public class PlayPrevious : IReturn<SuccessResponse>
    {
    }

    [Route("/player/next", "GET")]
    public class PlayNext : IReturn<SuccessResponse>
    {
    }

    [Route("/player/play", "GET")]
    public class PlaybackStart : IReturn<SuccessResponse>
    {
    }

    [Route("/player/stop", "GET")]
    public class PlaybackStop : IReturn<SuccessResponse>
    {
    }

    [Route("/player/pause", "GET")]
    public class PlaybackPause : IReturn<SuccessResponse>
    {
    }

    [Route("/player/playpause", "PUT")]
    public class PlaybackPlayPause : IReturn<SuccessResponse>
    {
    }

    [Route("/player/status", "GET")]
    public class GetPlayerStatus : IReturn<PlayerStatus>
    {
    }

    [Route("/player/playstate", "GET")]
    public class GetPlayState : IReturn<ValueResponse>
    {
    }

    [DataContract]
    public class SuccessStatusResponse : SuccessResponse
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// A response that returns the status of a functionality (enabled/disabled)
    /// </summary>
    [DataContract]
    public class StatusResponse
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }

    [DataContract]
    public class SuccessVolumeResponse : SuccessResponse
    {
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }

    [DataContract]
    public class VolumeResponse
    {
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }

    [DataContract]
    public class SuccessValueResponse : SuccessResponse
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    [DataContract]
    public class ValueResponse
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}