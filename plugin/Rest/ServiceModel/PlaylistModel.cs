#region Dependencies

using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{   
    [DataContract]
    public class CreatePlaylist
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "list", IsRequired = false)]
        public string[] List { get; set; }
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