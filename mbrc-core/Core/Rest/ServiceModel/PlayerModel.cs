using System.Runtime.Serialization;
using MusicBeeRemote.Core.Enumerations;
using MusicBeeRemote.Core.Rest.ServiceModel.Enum;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MusicBeeRemote.Core.Rest.ServiceModel
{
    [DataContract]
    public class SetShuffleState
    {
        [DataMember(Name = "status", IsRequired = true)]
        public Enumerations.ShuffleState? Status { get; set; }
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
    public class ShuffleStateResponse : ResponseBase
    {
        [DataMember(Name = "state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ShuffleState State { get; set; }
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
        [JsonConverter(typeof(StringEnumConverter))]
        public Enumerations.ShuffleState State { get; set; }
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