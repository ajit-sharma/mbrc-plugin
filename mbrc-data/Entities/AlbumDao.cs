namespace MusicBeeRemoteData.Entities
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an album entry stored in the library.
    /// </summary>
    [DataContract(Name = "library_album")]
    public class AlbumDao : TypeBase
    {        
        /// <summary>
        /// Backing field of the property <see cref="Name"/>
        /// </summary>
        private string _name;

        /// <summary>
        /// The id of the album artist.
        /// </summary>
        [DataMember(Name = "artist_id")]
        public int ArtistId { get; set; }

        /// <summary>
        /// The id of the album cover.
        /// </summary>
        [DataMember(Name = "cover_id")]
        public int CoverId { get; set; }

        /// <summary>
        /// The name (title) of the album.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get => _name;

            set => _name = string.IsNullOrEmpty(value) ? "[Empty]" : value;
        }
    }
}