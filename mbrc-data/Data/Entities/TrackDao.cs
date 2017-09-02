using System;
using System.Runtime.Serialization;

namespace MusicBeeRemote.Data.Entities
{
    /// <summary>
    ///     A track stored in the library.
    /// </summary>
    [DataContract]
    public class TrackDao : TypeBase, IComparable<TrackDao>
    {
        /// <summary>
        ///     The id of the album artist related with the album the track is part of.
        /// </summary>
        [DataMember(Name = "album_artist_id")]
        public int AlbumArtistId { get; set; }

        /// <summary>
        ///     The id of the album the track is part of.
        /// </summary>
        [DataMember(Name = "album_id")]
        public int AlbumId { get; set; }

        /// <summary>
        ///     The id of the track's artist.
        /// </summary>
        [DataMember(Name = "artist_id")]
        public int ArtistId { get; set; }

        /// <summary>
        ///     The disc field
        /// </summary>
        [DataMember(Name = "disc")]
        public int Disc { get; set; }

        /// <summary>
        ///     The id of the track's genre.
        /// </summary>
        [DataMember(Name = "genre_id")]
        public int GenreId { get; set; }

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
        public int CompareTo(TrackDao other)
        {
            var oIndex = other.Position;
            return oIndex == Position ? 0 : oIndex > Position ? -1 : 1;
        }
    }
}