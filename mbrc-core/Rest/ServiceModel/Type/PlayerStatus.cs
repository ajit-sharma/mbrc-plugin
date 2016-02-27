namespace MusicBeeRemoteCore.Rest.ServiceModel.Type
{
    using System.Runtime.Serialization;

    using MusicBeeRemoteCore.AndroidRemote.Enumerations;

    [DataContract]
    public class PlayerStatus : ResponseBase
    {
        [DataMember(Name = "mute")]
        public bool Mute { get; set; }

        [DataMember(Name = "state")]
        public string PlayerState { get; set; }

        [DataMember(Name = "repeat")]
        public string Repeat { get; set; }

        [DataMember(Name = "scrobble")]
        public bool Scrobble { get; set; }

        [DataMember(Name = "shuffle")]
        public Shuffle Shuffle { get; set; }

        [DataMember(Name = "volume")]
        public int Volume { get; set; }
    }
}