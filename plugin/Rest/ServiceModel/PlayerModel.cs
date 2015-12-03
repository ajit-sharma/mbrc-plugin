#region

using System.ComponentModel;
using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Enum;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route(Routes.PlayerShuffle, Verbs.Get, Summary = Summary.ShuffleGet)]
    public class GetShuffleState : IReturn<ShuffleResponse>
    {
    }

    [Route(Routes.PlayerShuffle, Verbs.Put, Summary = Summary.ShufflePut)]
    [DataContract]
    public class SetShuffleState : IReturn<ShuffleState>
    {
        [DataMember(Name = "status", IsRequired = true)]
        [Description(Descriptions.ShuffleState)]
        public AndroidRemote.Enumerations.ShuffleState? Status { get; set; }
    }

    [Route(Routes.PlayerScrobble, Verbs.Get, Summary = Summary.ScrobbleGet)]
    public class GetScrobbleStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerScrobble, Verbs.Put, Summary = Summary.ScrobbleSet)]
    [DataContract]
    public class SetScrobbleStatus : IReturn<StatusResponse>
    {
        [DataMember(Name = "enabled", IsRequired = false)]
        [Description(Descriptions.ScrobbleEnabled)]
        public bool? Enabled { get; set; }
    }


    [Route(Routes.PlayerRepeat, Verbs.Get, Summary = Summary.GetRepeat)]
    public class GetRepeatMode : IReturn<ValueResponse>
    {
    }

    [Route(Routes.PlayerRepeat, Verbs.Put, Summary = Summary.RepeatPut)]
    [DataContract]
    public class SetRepeatMode : IReturn<ResponseBase>
    {
	    [DataMember(Name = "mode", IsRequired = false)]
        [Description(Descriptions.RepeatMode)]
        public ApiRepeatMode? Mode { get; set; }
    }

    [Route(Routes.PlayerMute, Verbs.Get, Summary = Summary.MuteGet)]
    public class GetMuteStatus : IReturn<StatusResponse>
    {
    }

    [Route(Routes.PlayerMute, Verbs.Put, Summary = Summary.MutePut)]
    [DataContract]
    public class SetMuteStatus : IReturn<StatusResponse>
    {
	    [DataMember(Name="enabled",IsRequired = false)]
        [Description(Descriptions.Mute)]
        public bool? Enabled { get; set; }
    }


    [Route(Routes.PlayerVolume, Verbs.Get, Summary = Summary.VolumeGet)]
    public class GetVolume : IReturn<VolumeResponse>
    {
    }

    [Route(Routes.PlayerVolume, Verbs.Put, Summary = Summary.VolumePut)]
    [DataContract]
    public class SetVolume : IReturn<ResponseBase>
    {
	    [DataMember(Name = "value", IsRequired = true)]
        [ApiAllowableValues("value", 0 , 100)]
        public int Value { get; set; }
    }

    [Route(Routes.PlayerAction, Verbs.Get, Summary = Summary.PlayerAction)]
    [DataContract]
    public class PlayerAction : IReturn<ResponseBase>
    {
        [DataMember(Name = "action", IsRequired = true)]
        [Description(Descriptions.PlayerAction)]
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

    [Route(Routes.PlayerOutput, Verbs.Get, Summary = Summary.OutputGet)]
    public class GetOutputDevices : IReturn<OutputDeviceResponse>
    {
    }

    [Route(Routes.PlayerOutput, Verbs.Put, Summary = Summary.OutputPut)]
    [DataContract]
    public class PutOutputDevice : IReturn<OutputDeviceResponse>
    {
        [DataMember(Name = "active", IsRequired = true)]
        [Description(Descriptions.ActiveOutput)]
        public string Active { get; set; }
    }

    [DataContract]
    public class OutputDeviceResponse : ResponseBase
    {
        [DataMember(Name = "devices")]
        public string[] Devices { get; set; }
        [DataMember(Name = "active")]
        public string Active { get; set; }
    
    }

    [DataContract]
    public class ShuffleState : ResponseBase
    {
        [DataMember(Name = "state")]
        public AndroidRemote.Enumerations.ShuffleState State { get; set; }
    }

    /// <summary>
    ///     A response that returns the status of a functionality (enabled/disabled)
    /// </summary>
    [DataContract]
    public class StatusResponse : ResponseBase
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }

    [DataContract]
    public class ShuffleResponse : ResponseBase
    {
        [DataMember(Name = "state")]
        public AndroidRemote.Enumerations.ShuffleState State { get; set; } 
    }
    
    [DataContract]
    public class VolumeResponse : ResponseBase
    {
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }

    [DataContract]
    public class ValueResponse : ResponseBase
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}