namespace MusicBeePlugin.Rest.ServiceModel
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CreatePlaylist
    {
        [DataMember(Name = "list", IsRequired = false)]
        public string[] List { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }
    }

    [DataContract]
    public class PlaylistPlay
    {
        [DataMember(Name = "path", IsRequired = true)]
        public string Path { get; set; }
    }

    [DataContract]
    public class AddPlaylistTracks
    {
        [DataMember(Name = "list", IsRequired = true)]
        public string[] List { get; set; }
    }
}