using System.Collections.Generic;

namespace MusicBeeRemote.Core.Feature.Library
{
    public class CompareableAlbum
    {
        private sealed class NameArtistEqualityComparer : IEqualityComparer<CompareableAlbum>
        {
            public bool Equals(CompareableAlbum x, CompareableAlbum y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Name, y.Name) && string.Equals(x.Artist, y.Artist);
            }

            public int GetHashCode(CompareableAlbum obj)
            {
                unchecked
                {
                    return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ (obj.Artist != null ? obj.Artist.GetHashCode() : 0);
                }
            }
        }

        public static IEqualityComparer<CompareableAlbum> NameArtistComparer { get; } = new NameArtistEqualityComparer();

        public int Id { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }       
    }
}