using System.Runtime.Serialization;
using MusicBeeRemote.Core.Feature.Player;

namespace MusicBeeRemote.Core.Network.Http.Responses.Type
{
    [DataContract]
    public class PlayerStatus
    {
        [DataMember(Name = "playerrepeat")]
        public Repeat RepeatMode { get; set; }

        [DataMember(Name = "playermute")]
        public bool Mute { get; set; }

        [DataMember(Name = "playershuffle")]
        public ShuffleState Shuffle { get; set; }

        [DataMember(Name = "scrobbler")]
        public bool Scrobbling { get; set; }

        [DataMember(Name = "playerstate")]
        public PlayerState PlayerState { get; set; }

        [DataMember(Name = "playervolume")]
        public int Volume { get; set; }
    }
}