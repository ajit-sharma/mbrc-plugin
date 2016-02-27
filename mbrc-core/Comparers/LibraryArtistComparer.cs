namespace MusicBeeRemoteCore.Comparers
{
    using System.Collections.Generic;

    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// Compares Two <see cref="LibraryArtist"/>s
    /// </summary>
    public class LibraryArtistComparer : IEqualityComparer<LibraryArtist>
    {
        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <paramref name="LibraryArtist" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="LibraryArtist" /> to compare.</param>
        public bool Equals(LibraryArtist x, LibraryArtist y)
        {
            if (x == null && y == null || ReferenceEquals(x, null))
            {
                return false;
            }

            return ReferenceEquals(x, y) || x.Name.Equals(y.Name);
        }

        /// <summary>
        ///     Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        ///     A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="LibraryArtist" /> for which a hash code is to be returned.</param>
        public int GetHashCode(LibraryArtist obj)
        {
            return obj?.Name?.GetHashCode() ?? 0;
        }
    }
}