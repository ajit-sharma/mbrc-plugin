#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	/// <summary>
	/// Represents an album entry stored in the library.
	/// </summary>
	[DataContract(Name = "libraryAlbum")]
    [Alias("LibraryAlbum")]
    public class LibraryAlbum : TypeBase
    {
		/// <summary>
		/// Creates a new LibraryAlbum with an empty <see cref="TrackList"/> 
		/// </summary>
		public LibraryAlbum()
        {
            TrackList = new List<LibraryTrack>();
        }

		/// <summary>
		/// The name (title) of the album.
		/// </summary>
		[DataMember(Name = "name")]
        public string Name { get; set; }

		/// <summary>
		/// The id of the album artist.
		/// </summary>
		[References(typeof (LibraryArtist))]
        [DataMember(Name = "artistId")]
        public long ArtistId { get; set; }

		/// <summary>
		/// The id of the album cover.
		/// </summary>
		[DataMember(Name = "coverId")]
        public long CoverId { get; set; }

		/// <summary>
		/// Unique album identifier retrieved from MusicBee.
		/// </summary>
		[DataMember(Name = "albumId")]
        public string AlbumId { get; set; }

		/// <summary>
		/// A list with the tracks contained in the album.
		/// </summary>
		[Ignore]
        [IgnoreDataMember]
        public List<LibraryTrack> TrackList { get; set; }
    }
}