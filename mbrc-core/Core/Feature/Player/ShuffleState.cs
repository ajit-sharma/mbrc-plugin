using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Feature.Player
{
    public enum ShuffleState
    {
        [EnumMember(Value = "off")]
        Off,
        [EnumMember(Value = "shuffle")]
        Shuffle,
        [EnumMember(Value = "autodj")]
        Autodj
    }
}
