using System;
using System.Runtime.Serialization;

namespace MusicBeeRemote.Core.Feature.Library
{
    [DataContract]
    public class Album : IEquatable<Album>, IComparable<Album>
    {

        public Album(string artist, string name)
        {
            Name = name;
            Artist = artist;
            TrackCount = 1;
        }

        [DataMember(Name = "album")]
        public string Name { get; set; }

        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        public void IncreaseCount()
        {
            TrackCount++;
        }

        [DataMember(Name = "count")]
        public int TrackCount { get; private set; }

        public bool Equals(Album other)
        {
            return other != null && other.Artist.Equals(Artist) && other.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Artist.GetHashCode() ^ Name.GetHashCode();
        }

        public int CompareTo(Album other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
            return nameComparison != 0 
                ? nameComparison 
                : string.Compare(Artist, other.Artist, StringComparison.Ordinal);
        }
    }
}
