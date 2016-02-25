namespace MusicBeeRemoteCore.Rest.ServiceModel
{
    using System;
    using System.Runtime.Serialization;
    using AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.Rest.ServiceModel.Enum;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    [DataContract]
    public class SetShuffleState
    {
        [DataMember(Name = "status", IsRequired = true)]
        public MusicBeeRemoteCore.AndroidRemote.Enumerations.Shuffle? Status { get; set; }
    }

    [DataContract]
    public class SetScrobbleStatus
    {
        [DataMember(Name = "enabled", IsRequired = false)]
        public bool? Enabled { get; set; }
    }

    [DataContract]
    public class SetRepeatMode
    {
        [DataMember(Name = "mode", IsRequired = false)]
        public ApiRepeatMode? Mode { get; set; }
    }

    [DataContract]
    public class SetMuteStatus
    {
        [DataMember(Name = "enabled", IsRequired = false)]
        public bool? Enabled { get; set; }
    }

    [DataContract]
    public class SetVolume
    {
        [DataMember(Name = "value", IsRequired = true)]
        public int Value { get; set; }
    }

    [DataContract]
    public class PutOutputDevice
    {
        [DataMember(Name = "active", IsRequired = true)]
        public string Active { get; set; }
    }

    [DataContract]
    public class OutputDeviceResponse : ResponseBase
    {
        [DataMember(Name = "active")]
        public string Active { get; set; }

        [DataMember(Name = "devices")]
        public string[] Devices { get; set; }
    }

    [DataContract]
    public class ShuffleState : ResponseBase
    {
        [DataMember(Name = "state")]
        public MusicBeeRemoteCore.AndroidRemote.Enumerations.Shuffle State { get; set; }

        public static explicit operator ShuffleState(AndroidRemote.Enumerations.Shuffle? v)
        {
            throw new NotImplementedException();
        }
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
        public MusicBeeRemoteCore.AndroidRemote.Enumerations.Shuffle State { get; set; }
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