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
        #region Player Route API
        public const string PlayerShuffle = "/player/shuffle";
        public const string PlayerScrobble = "/player/scrobble";
        public const string PlayerRepeat = "/player/repeat";
        public const string PlayerMute = "/player/mute";
        public const string PlayerVolume = "/player/volume";
        public const string PlayerAutodj = "/player/autodj";
        public const string PlayerPlaystate = @"/player/playstate";
        public const string PlayerPrevious = "/player/previous";
        public const string PlayerNext = "/player/next";
        public const string PlayerPlay = "/player/play";
        public const string PlayerStop = "/player/stop";
        public const string PlayerPause = "/player/pause";
        public const string PlayerPlaypause = "/player/playpause";
        public const string PlayerStatus = "/player/status";
        #endregion
    }

    /// <summary>
    ///     Short Summary of the API functionality
    /// </summary>
    public class Summary
    {
        public const string ShuffleGet = "Gets the current state of shuffle.";
        public const string ShufflePut = @"Sets the shuffle status.";
        public const string ScrobbleGet = "Gets the status of last.fm scrobbling";
        public const string ScrobbleSet = "Sets the status of last.fm scrobbling.";
    }

    /// <summary>
    ///     Holds descriptions of various parameters
    /// </summary>
    public class Description
    {
        public const string Mute = @"If the value is true or false it will mute/unmute the audio." +
            "\n If left empty it will toggle mute on/off depending on the previous state.";

        public const string ShuffleEnabled = @"If the value is true or false it will enable/disable shuffle." +
                                             "\n If left empty it will toggle shuffle on/off depending on the previous state.";

        public const string ScrobbleEnabled =
            @"If the value is true or false it will enable/disable last.fm scrobbling." +
            "\n If left empty it will toggle scrobbling on/off depending on the previous state.";

        public const string RepeatMode =
            @"It can be all/none or empty. If left empty it will change between the available states."
            + @" [Note: As far as I know repeat one is not supported by the MusicBee API.";

        public const string Volume = @"The new volume of the player ranges from 0 (no sound) to 100 (maximum)";
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

    [Route(Routes.PlayerScrobble, Verbs.Get, Summary = Summary.ScrobbleGet)]
    public class GetScrobbleStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerScrobble, Verbs.Put, Summary = Summary.ScrobbleSet)]
    public class SetScrobbleStatus : IReturn<SuccessStatusResponse>
    {
        [ApiMember(Name = "enabled", ParameterType = "query", DataType = "boolean",
            Description = Description.ScrobbleEnabled, IsRequired = false)]
        public bool? enabled { get; set; }
    }


    [Route(Routes.PlayerRepeat, Verbs.Get)]
    public class GetRepeatMode : IReturn<ValueResponse>
    {
    }

    [Route(Routes.PlayerRepeat, Verbs.Put)]
    public class SetRepeatMode : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "mode", ParameterType = "query", DataType = "string",
            Description = Description.RepeatMode, IsRequired = false)]
        [ApiAllowableValues("mode", "all", "none")]
        public string mode { get; set; }
    }

    [Route(Routes.PlayerMute, Verbs.Get)]
    public class GetMuteStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerMute, Verbs.Put)]
    public class SetMuteStatus : IReturn<SuccessStatusResponse>
    {
        [ApiMember(Name="enabled", ParameterType = "query", DataType = "boolean", IsRequired = false,
            Description = Description.Mute)]
        public bool? enabled { get; set; }
    }


    [Route(Routes.PlayerVolume, Verbs.Get)]
    public class GetVolume : IReturn<VolumeResponse>
    {
    }

    [Route(Routes.PlayerVolume, Verbs.Put)]
    public class SetVolume : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "value",ParameterType = "query", DataType = "integer", IsRequired = true,
            Description = Description.Volume)]
        [ApiAllowableValues("value", 0 , 100)]
        public int value { get; set; }
    }

    [Route(Routes.PlayerAutodj, Verbs.Get)]
    public class GetAutoDjStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerAutodj, Verbs.Put)]
    public class SetAutoDjStatus : IReturn<SuccessStatusResponse>
    {
        public bool enabled { get; set; }
    }

    [Route(Routes.PlayerPrevious, Verbs.Get)]
    public class PlayPrevious : IReturn<SuccessResponse>
    {
    }

    [Route(Routes.PlayerNext, Verbs.Get)]
    public class PlayNext : IReturn<SuccessResponse>
    {
    }

    [Route(Routes.PlayerPlay, Verbs.Get)]
    public class PlaybackStart : IReturn<SuccessResponse>
    {
    }

    [Route(Routes.PlayerStop, Verbs.Get)]
    public class PlaybackStop : IReturn<SuccessResponse>
    {
    }

    [Route(Routes.PlayerPause, Verbs.Get)]
    public class PlaybackPause : IReturn<SuccessResponse>
    {
    }

    [Route(Routes.PlayerPlaypause, Verbs.Put)]
    public class PlaybackPlayPause : IReturn<SuccessResponse>
    {
    }

    [Route(Routes.PlayerStatus, Verbs.Get)]
    public class GetPlayerStatus : IReturn<PlayerStatus>
    {
    }

    [Route(Routes.PlayerPlaystate, Verbs.Get)]
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
    ///     A response that returns the status of a functionality (enabled/disabled)
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