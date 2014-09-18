using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    [DataContract]
    public class LibraryArtist
    {
        public LibraryArtist(string name)
        {
            Name = name;
        }

        public LibraryArtist()
        {
        }

        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        [Index(Unique = true)]
        public string Name { get; set; }

        [DataMember(Name = "imageUrl")]
        public string ImageUrl { get; set; }
    }
}