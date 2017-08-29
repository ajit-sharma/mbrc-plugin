using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Feature.Library
{
    [DataContract]
    public class Artist
    {
        [DataMember(Name = "artist")]
        public string Name { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "image")]
        public string ImageUrl { get; set; }


        public Artist(string name, int count)
        {
            Name = name;
            Count = count;
        }
    }
}