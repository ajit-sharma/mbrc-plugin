using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    public class LibraryAlbum
    {
        public LibraryAlbum(string name)
        {
            this.name = name;
            TrackList = new List<LibraryTrack>();
        }

        public LibraryAlbum()
        {
            TrackList = new List<LibraryTrack>();
        }

        [AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        [References(typeof(LibraryArtist))]
        public int artist_id { get; set; }
        public string cover_hash { get; set; }

        public string album_id { get; set; }

        [Ignore]
        public List<LibraryTrack> TrackList { get; set; }

    }
}