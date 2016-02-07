#region

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	/// <summary>
	/// Represents an album entry stored in the library.
	/// </summary>
	[DataContract(Name = "library_album")]
    public class LibraryAlbum : TypeBase
    {
		/// <summary>
		/// Backing field of the property <see cref="Name"/>
		/// </summary>
		private string _name;

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
		public string Name
		{
			get { return _name; }
			set { _name = string.IsNullOrEmpty(value) ? "[Empty]" : value; }
		}

		/// <summary>
		/// The id of the album artist.
		/// </summary>
		
        [DataMember(Name = "artist_id")]
        public long ArtistId { get; set; }

		/// <summary>
		/// The id of the album cover.
		/// </summary>
		[DataMember(Name = "cover_id")]
        public long CoverId { get; set; }

		/// <summary>
		/// Unique album identifier retrieved from MusicBee.
		/// </summary>
		[DataMember(Name = "album_id")]
        public string AlbumId { get; set; }

		/// <summary>
		/// A list with the tracks contained in the album.
		/// </summary>
        [IgnoreDataMember]
        public List<LibraryTrack> TrackList { get; set; }
    }
    
}