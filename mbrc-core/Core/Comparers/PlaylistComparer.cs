using System.Collections.Generic;
using MusicBeeRemote.Data.Entities;

namespace MusicBeeRemote.Core.Comparers
{
    /// <summary>
    ///     Custom <c>EqualityComparer</c> for <see cref="Playlist" /> objects.
    ///     Since adding mutable properties in overridden GetHashCode is not
    ///     recommended.
    /// </summary>
    internal class PlaylistComparer : IEqualityComparer<Playlist>
    {
        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <see cref="Playlist"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="Playlist"/> /> to compare.</param>
        public bool Equals(Playlist x, Playlist y)
        {
            if (x == null && y == null || ReferenceEquals(x, null))
            {
                return false;
            }

            return ReferenceEquals(x, y) || x.Url.Equals(y.Url);
        }

        /// <summary>
        ///     Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///     A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="Playlist" /> for which a hash code is to be returned.</param>
        public int GetHashCode(Playlist obj)
        {
            return obj?.Url?.GetHashCode() ?? 0;
        }
    }
}