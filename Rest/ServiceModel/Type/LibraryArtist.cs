using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    public class LibraryArtist
    {
        public LibraryArtist(string name)
        {
            this.name = name;
        }

        public LibraryArtist()
        {
            
        }

        [AutoIncrement]
        public int id { get; set; }
        [Index(Unique = true)]
        public string name { get; set; }
        public string image_url { get; set; }

    }
}