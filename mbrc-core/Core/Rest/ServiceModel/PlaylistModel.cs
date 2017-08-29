using System.Runtime.Serialization;
using MusicBeeRemote.Core.Enumerations;

namespace MusicBeeRemote.Core.Rest.ServiceModel
{
    [DataContract]
    public class CreatePlaylist
    {
        [DataMember(Name = "list", IsRequired = false)]
        public string[] List { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "id", IsRequired = false)]
        public long Id { get; set; }

        [DataMember(Name = "type", IsRequired = false)]
        public MetaTag Type { get; set; }
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
        [DataMember(Name = "list", IsRequired = false)]
        public string[] List { get; set; }

        [DataMember(Name = "id", IsRequired = false)]
        public long Id { get; set; }

        [DataMember(Name = "type", IsRequired = false)]
        public MetaTag Type { get; set; }
    }
}