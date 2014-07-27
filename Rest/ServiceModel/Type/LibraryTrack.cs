using System;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    public class LibraryTrack:IComparable<LibraryTrack>
    {
        [AutoIncrement]
        public int id { get; set; }
        public string title { get; set; }

        public int index { get; set; }
        [References(typeof(LibraryGenre))]
        public int genre_id { get; set; }
        [References(typeof(LibraryArtist))]
        public int artist_id { get; set; }
        [References(typeof(LibraryArtist))]
        public int album_artist_id { get; set; }
        [References(typeof(LibraryAlbum))]
        public int album_id { get; set; }
        public string year { get; set; }
        public string path { get; set; }
        public int CompareTo(LibraryTrack other)
        {
            var oIndex = other.index;
            return oIndex == index ? 0 : oIndex > index ? -1 : 1;
        }
    }
}