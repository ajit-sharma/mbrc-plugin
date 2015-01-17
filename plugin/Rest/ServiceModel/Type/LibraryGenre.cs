using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    /// <summary>
    /// A representation of a Music Genre.
    /// </summary>
    [DataContract]
    [Alias("LibraryGenre")]
    public class LibraryGenre
    {
        /// <summary>
        /// Parametrized constructor that creates a new LibraryGenre with the
        /// supplied <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the new genre.</param>
        public LibraryGenre(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Default constructor for the creation of an empty genre.
        /// </summary>
        public LibraryGenre()
        {
        }

        /// <summary>
        /// The id of the genre in the database.
        /// </summary>
        [AutoIncrement]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// The name of the genre. The name must be unique.
        /// </summary>
        [Index(Unique = true)]
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}