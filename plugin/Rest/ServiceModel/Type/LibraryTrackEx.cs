using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    /// <summary>
    /// The complete form of the <see cref="LibraryTrack"/> class.
    /// It contains the values in the place of ids.
    /// </summary>
    public class LibraryTrackEx
    {
	    /// <summary>
	    /// Default constructor to create a <see cref="LibraryTrackEx"/>
	    /// </summary>
	    public LibraryTrackEx()
	    {
		    
	    }

	    /// <summary>
	    /// Creates a new <see cref="LibraryTrackEx" /> taking information from the
	    /// metadata available in the <paramref name="tags"/> array.
	    /// </summary>
	    /// <param name="tags">
	    /// An array containing the metadata tags for the track
	    /// </param>
	    public LibraryTrackEx(string[] tags)
	    {
		    var i = 0;
		    Artist = tags[i++];
		    AlbumArtist = tags[i++];
		    Album = tags[i++];
			Genre = tags[i++];
		    Title = tags[i++];
		    Year = tags[i++];

			var trackNo = tags[i];
			int position;
			int.TryParse(trackNo, out position);
		    Position = position;

	    }
        /// <summary>
        /// The id of the <see cref="LibraryTrack"/> entry.
        /// </summary>
        [BelongTo(typeof(LibraryTrack))]
        public long Id { get; set; }

        /// <summary>
        /// Represents the title of the track.
        /// </summary>
        [BelongTo(typeof(LibraryTrack))]
        public string Title { get; set; }

        /// <summary>
        /// Represents the position of the track in the album.
        /// </summary>
        [BelongTo(typeof(LibraryTrack))]
        public int Position { get; set; }

        /// <summary>
        /// Represents the name of the track's genre.
        /// </summary>
        [BelongTo(typeof(LibraryGenre))]
        [Alias("Name")]
        public string Genre { get; set; }

        /// <summary>
        /// Represents the name of the track's artist.
        /// </summary>
        [BelongTo(typeof(LibraryArtist))]
        [Alias("Name")]
        public string Artist { get; set; }

        /// <summary>
        /// Represents the name of the artist that recorded the album the track
        /// is part of.
        /// </summary>
        [BelongTo(typeof(LibraryArtist))]
        [Alias("Name")]
        public string AlbumArtist { get; set; }

        /// <summary>
        /// Represents the name of the album the track is part of.
        /// </summary>
        [BelongTo(typeof(LibraryAlbum))]
        [Alias("Name")]
        public string Album { get; set; }

        /// <summary>
        /// A string representation of the date or year the album was released.
        /// </summary>
        [BelongTo(typeof(LibraryTrack))]
        public string Year { get; set; }

        /// <summary>
        /// The path of the music file in the file system.
        /// </summary>
        [BelongTo(typeof(LibraryTrack))]
        public string Path { get; set; }
    }
}