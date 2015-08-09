#region

using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Enum;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Api.Swagger;
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
        [ApiMember(Name = "enabled", ParameterType = "query", DataType = SwaggerType.Boolean,
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
        [ApiMember(Name = "enabled", ParameterType = "query", DataType = SwaggerType.Boolean,
            Description = Description.ScrobbleEnabled, IsRequired = false)]
        public bool? Enabled { get; set; }
    }


    [Route(Routes.PlayerRepeat, Verbs.Get, Summary = Summary.GetRepeat)]
    public class GetRepeatMode : IReturn<ValueResponse>
    {
    }

    [Route(Routes.PlayerRepeat, Verbs.Put, Summary = Summary.RepeatPut)]
    public class SetRepeatMode : IReturn<SuccessResponse>
    {
	    [ApiMember(Name = "mode", ParameterType = "query", DataType = SwaggerType.String,
            Description = Description.RepeatMode, IsRequired = false)]
        [ApiAllowableValues("mode", "all", "none")]
        public string Mode { get; set; }
    }

    [Route(Routes.PlayerMute, Verbs.Get, Summary = Summary.MuteGet)]
    public class GetMuteStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerMute, Verbs.Put, Summary = Summary.MutePut)]
    public class SetMuteStatus : IReturn<SuccessStatusResponse>
    {
	    [ApiMember(Name="enabled", ParameterType = "query", DataType = SwaggerType.Boolean, IsRequired = false,
            Description = Description.Mute)]
        public bool? Enabled { get; set; }
    }


    [Route(Routes.PlayerVolume, Verbs.Get, Summary = Summary.VolumeGet)]
    public class GetVolume : IReturn<VolumeResponse>
    {
    }

    [Route(Routes.PlayerVolume, Verbs.Put, Summary = Summary.VolumePut)]
    public class SetVolume : IReturn<SuccessResponse>
    {
	    [ApiMember(Name = "value",ParameterType = "query", DataType = SwaggerType.Int, IsRequired = true,
            Description = Description.Volume)]
        [ApiAllowableValues("value", 0 , 100)]
        public int Value { get; set; }
    }

    [Route(Routes.PlayerAutodj, Verbs.Get, Summary = Summary.AutoDjGet)]
    public class GetAutoDjStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerAutodj, Verbs.Put, Summary = Summary.AutoDjPut)]
    public class SetAutoDjStatus : IReturn<SuccessStatusResponse>
    {
	    [ApiMember(Name = "enabled", ParameterType = "query", DataType = SwaggerType.Boolean, IsRequired = false,
			Description = Description.AutoDjPut)]
        public bool? Enabled { get; set; }
    }

    [Route(Routes.PlayerAction, Verbs.Put, Summary = Summary.PlayerAction)]
    public class PlayerAction : IReturn<SuccessResponse>
    {
	    [ApiMember(Name = "action", ParameterType = "query", DataType = SwaggerType.String, IsRequired = true,
			Description = Description.PlayerAction)]
		[ApiAllowableValues("action", typeof(PlaybackAction))]
		public PlaybackAction Action { get; set; }
		
    }

    [Route(Routes.PlayerStatus, Verbs.Get, Summary = Summary.PlayerStatusGet)]
    public class GetPlayerStatus : IReturn<PlayerStatus>
    {
    }

    [Route(Routes.PlayerPlaystate, Verbs.Get, Summary = Summary.PlaystateGet)]
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