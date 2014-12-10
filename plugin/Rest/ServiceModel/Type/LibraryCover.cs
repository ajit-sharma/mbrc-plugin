using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class LibraryCover
    {
        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember (Name = "hash")]
        public string Hash { get; set; }
    }
}
