using System.Runtime.Serialization;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class Track
    {
        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "album")]
        public string Album { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }
    }
}