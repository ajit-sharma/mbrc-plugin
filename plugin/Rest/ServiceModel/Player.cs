#region

using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route(Routes.PlayerShuffle, Verbs.Get, Summary = Summary.ShuffleGet)]
    public class GetShuffleState : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerShuffle, Verbs.Put, Summary = Summary.ShufflePut)]
    public class SetShuffleState : IReturn<SuccessStatusResponse>
    {
        [ApiMember(Name = "enabled", ParameterType = "query", DataType = "boolean",
            Description = Description.ShuffleEnabled, IsRequired = false)]
        public bool? Enabled { get; set; }
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
        public bool? Enabled { get; set; }
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
        public string Mode { get; set; }
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
        public bool? Enabled { get; set; }
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
        public int Value { get; set; }
    }

    [Route(Routes.PlayerAutodj, Verbs.Get)]
    public class GetAutoDjStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerAutodj, Verbs.Put)]
    public class SetAutoDjStatus : IReturn<SuccessStatusResponse>
    {
        public bool Enabled { get; set; }
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