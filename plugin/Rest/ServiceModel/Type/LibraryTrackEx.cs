using System.Collections.Generic;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    /// <summary>
    ///     The complete form of the <see cref="LibraryTrack" /> class.
    ///     It contains the values in the place of ids.
    /// </summary>
    public class LibraryTrackEx
    {
        /// <summary>
        ///     Default constructor to create a <see cref="LibraryTrackEx" />
        /// </summary>
        public LibraryTrackEx()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="LibraryTrackEx" /> taking information from the
        ///     metadata available in the <paramref name="tags" /> array.
        /// </summary>
        /// <param name="tags">
        ///     An array containing the metadata tags for the track
        /// </param>
        public LibraryTrackEx(IList<string> tags)
        {
            var i = 0;
            Artist = tags[i++];
            AlbumArtist = tags[i++];
            Album = tags[i++];
            Genre = tags[i++];
            Title = tags[i++];
            Year = tags[i++];

            var trackNo = tags[i++];
            var discNo = tags[i];
            int position;
            int disc;
            int.TryParse(trackNo, out position);
            int.TryParse(discNo, out disc);
            Position = position;
            Disc = disc;
        }

        /// <summary>
        ///     The id of the <see cref="LibraryTrack" /> entry.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Represents the title of the track.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Represents the position of the track in the album.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        ///     Represents the name of the track's genre.
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        ///     Represents the name of the track's artist.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        ///     Represents the name of the artist that recorded the album the track
        ///     is part of.
        /// </summary>
        public string AlbumArtist { get; set; }

        /// <summary>
        ///     Represents the name of the album the track is part of.
        /// </summary>
        public string Album { get; set; }

        /// <summary>
        ///     A string representation of the date or year the album was released.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        ///     The path of the music file in the file system.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     The disc number of the album
        /// </summary>
        public int Disc { get; set; }
    }
}