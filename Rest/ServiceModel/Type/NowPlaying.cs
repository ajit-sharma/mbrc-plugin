using System.Runtime.Serialization;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class NowPlaying
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "index")]
        public int Index { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }
    }
}