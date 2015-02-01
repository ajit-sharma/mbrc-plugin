using System;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	/// <summary>
	/// A track stored in the library.
	/// </summary>
	[DataContract]
    public class LibraryTrack : TypeBase, IComparable<LibraryTrack>
    {
		/// <summary>
		/// The title of the track.
		/// </summary>
		[DataMember(Name = "title")]
        public string Title { get; set; }

		/// <summary>
		/// The position of the track in the album.
		/// </summary>
		[DataMember(Name = "position")]
        public int Position { get; set; }

		/// <summary>
		/// The id of the track's genre.
		/// </summary>
		[DataMember(Name = "genreId")]
        [References(typeof (LibraryGenre))]
        public long GenreId { get; set; }

		/// <summary>
		/// The id of the track's artist.
		/// </summary>
		[DataMember(Name = "artistId")]
        [References(typeof (LibraryArtist))]
        public long ArtistId { get; set; }

		/// <summary>
		/// The id of the album artist related with the album the track is part of.
		/// </summary>
		[DataMember(Name = "albumArtistId")]
        [References(typeof (LibraryArtist))]
        public long AlbumArtistId { get; set; }

		/// <summary>
		/// The id of the album the track is part of.
		/// </summary>
		[DataMember(Name = "albumId")]
        [References(typeof (LibraryAlbum))]
        public long AlbumId { get; set; }

		/// <summary>
		/// The year the track was released.
		/// </summary>
		[DataMember(Name = "year")]
        public string Year { get; set; }

		/// <summary>
		/// </summary>
		[DataMember(Name = "path")]
        public string Path { get; set; }

		/// <summary>
		/// Compares two tracks
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(LibraryTrack other)
        {
            var oIndex = other.Position;
            return oIndex == Position ? 0 : oIndex > Position ? -1 : 1;
        }
    }
}