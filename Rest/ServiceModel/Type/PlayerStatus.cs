#region

using System.Runtime.Serialization;

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
        public bool Shuffle { get; set; }

        [DataMember(Name = "scrobble")]
        public bool Scrobble { get; set; }

        [DataMember(Name = "state")]
        public string PlayerState { get; set; }

        [DataMember(Name = "volume")]
        public int Volume { get; set; }
    }
}