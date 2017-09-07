using System.Runtime.Serialization;
using MusicBeeRemote.Core.Feature.Player;
using MusicBeeRemote.Core.Network.Http.Responses.Enum;
using MusicBeeRemote.Core.Network.Http.Responses.Type;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MusicBeeRemote.Core.Network.Http.Responses
{
    [DataContract]
    public class SetShuffleState
    {
        [DataMember(Name = "status", IsRequired = true)]
        public ShuffleState? Status { get; set; }
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
    public class OutputDeviceResponse : ApiResponse
    {
        [DataMember(Name = "active")]
        public string Active { get; set; }

        [DataMember(Name = "devices")]
        public string[] Devices { get; set; }
    }

    [DataContract]
    public class ShuffleStateResponse : ApiResponse
    {
        [DataMember(Name = "state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ShuffleState State { get; set; }
    }

    /// <summary>
    ///     A response that returns the status of a functionality (enabled/disabled)
    /// </summary>
    [DataContract]
    public class StatusResponse : ApiResponse
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }

    [DataContract]
    public class ShuffleResponse : ApiResponse
    {
        [DataMember(Name = "state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ShuffleState State { get; set; }
    }

    [DataContract]
    public class VolumeResponse : ApiResponse
    {
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }

    [DataContract]
    public class ValueResponse : ApiResponse
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}