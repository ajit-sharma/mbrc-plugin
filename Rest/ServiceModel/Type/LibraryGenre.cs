using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class LibraryGenre
    {
        public LibraryGenre(string name)
        {
            Name = name;
        }

        public LibraryGenre()
        {
        }

        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Index(Unique = true)]
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}