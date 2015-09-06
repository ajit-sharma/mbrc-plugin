#region

using System.Runtime.Serialization;
using MusicBeePlugin.AndroidRemote.Enumerations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class PlayerStatus
    {
        [DataMember(Name = "repeat")]
        public string Repeat { get; set; }

        [DataMember(Name = "mute")]
        public bool Mute { get; set; }

        [DataMember(Name = "shuffle")]
        public ShuffleState Shuffle { get; set; }

        [DataMember(Name = "scrobble")]
        public bool Scrobble { get; set; }

        [DataMember(Name = "state")]
        public string PlayerState { get; set; }

        [DataMember(Name = "volume")]
        public int Volume { get; set; }
    }
}