using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    public class LibraryGenre
    {
        [AutoIncrement]
        public int id { get; set; }
        [Index(Unique = true)]
        public string name { get; set; }
        
        public LibraryGenre(string name)
        {
            this.name = name;
        }

        public LibraryGenre()
        {
            
        }
    }
}