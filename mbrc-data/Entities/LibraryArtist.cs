namespace MusicBeeRemoteData.Entities
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     This class represents an Artist with the information used in the
    ///     cache and API.
    /// </summary>
    [DataContract]
    public class LibraryArtist : TypeBase
    {
        /// <summary>
        /// Backing field for the <see cref="Name"/> property
        /// </summary>
        private string name;

        /// <summary>
        ///     Parametrized constructor of the LibraryArtist class. It creates a
        ///     new LibraryArtist with the supplied <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the artist created.</param>
        public LibraryArtist(string name)
        {
            this.Name = name;
        }

        /// <summary>
        ///     Default constructor of LibraryArtist class.
        /// </summary>
        public LibraryArtist()
        {
        }

        /// <summary>
        ///     The genre id property that represents the artist's genre.
        /// </summary>
        [DataMember(Name = "genre")]
        public long GenreId { get; set; }

        /// <summary>
        ///     A URL to the place the artist image is stored.
        /// </summary>
        [DataMember(Name = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        ///     The name property of the artist. The name should be unique.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = string.IsNullOrEmpty(value) ? "[Empty]" : value;
            }
        }
    }
}