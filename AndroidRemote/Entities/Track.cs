using System;

namespace MusicBeePlugin.AndroidRemote.Entities
{
    public class Track :IEquatable<Track>, IComparable<Track>
    {
        public Track(string artist, string title, string hash)
        {
            this.title = title;
            this.artist = artist;
            this.trackno = 0;
            this.index = 0;
            this.hash = hash;
        }

        public Track(string artist, string title, int trackNo, string hash)
        {
            this.artist = artist;
            this.title = title;
            this.index = 0;
            this.hash = hash;
            this.trackno = trackNo;
        }

        public int index { get; set; }

        public string hash { get; private set; }

        public string artist { get; private set; }

        public string title { get; private set; }

        public int trackno { get; private set; }

        public bool Equals(Track other)
        {
            return (other.artist.Equals(artist) && other.title.Equals(title));
        }

        public int CompareTo(Track other)
        {
            return other == null ? 1 : trackno.CompareTo(other.trackno);
        }
    }
}
