namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     A track stored in the library.
    /// </summary>
    [DataContract]
    public class LibraryTrack : TypeBase, IComparable<LibraryTrack>
    {
        /// <summary>
        ///     The id of the album artist related with the album the track is part of.
        /// </summary>
        [DataMember(Name = "album_artist_id")]
        public long AlbumArtistId { get; set; }

        /// <summary>
        ///     The id of the album the track is part of.
        /// </summary>
        [DataMember(Name = "album_id")]
        public long AlbumId { get; set; }

        /// <summary>
        ///     The id of the track's artist.
        /// </summary>
        [DataMember(Name = "artist_id")]
        public long ArtistId { get; set; }

        /// <summary>
        ///     The disc field
        /// </summary>
        [DataMember(Name = "disc")]
        public int Disc { get; set; }

        /// <summary>
        ///     The id of the track's genre.
        /// </summary>
        [DataMember(Name = "genre_id")]
        public long GenreId { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        /// <summary>
        ///     The position of the track in the album.
        /// </summary>
        [DataMember(Name = "position")]
        public int Position { get; set; }

        /// <summary>
        ///     The title of the track.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        ///     The year the track was released.
        /// </summary>
        [DataMember(Name = "year")]
        public string Year { get; set; }

        /// <summary>
        ///     Compares two tracks
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(LibraryTrack other)
        {
            var oIndex = other.Position;
            return oIndex == this.Position ? 0 : oIndex > this.Position ? -1 : 1;
        }
    }
}