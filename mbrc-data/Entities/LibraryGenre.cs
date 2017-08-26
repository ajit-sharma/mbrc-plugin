namespace MusicBeeRemoteData.Entities
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     A representation of a Music Genre.
    /// </summary>
    [DataContract]
    public class LibraryGenre : TypeBase
    {
        /// <summary>
        /// Backing field of the <see cref="Name"/> property.
        /// </summary>
        private string _name;

        /// <summary>
        ///     Parametrized constructor that creates a new LibraryGenre with the
        ///     supplied <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the new genre.</param>
        public LibraryGenre(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Default constructor for the creation of an empty genre.
        /// </summary>
        public LibraryGenre()
        {
        }

        /// <summary>
        ///     The name of the genre. The name must be unique.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get => _name;

            set => _name = string.IsNullOrEmpty(value) ? "[Empty]" : value;
        }
    }
}