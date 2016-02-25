namespace MusicBeeRemoteCore.Comparers
{
    using System.Collections.Generic;

    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// Used to compare to <see cref="LibraryGenre"/> objects.
    /// </summary>
    internal class LibraryGenreComparer : IEqualityComparer<LibraryGenre>
    {
        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">
        ///     The first object of type <paramref name="LibraryGenree" />
        ///     to compare.
        /// </param>
        /// <param name="y">
        ///     The second object of type
        ///     <paramref name="LibraryGenre" /> to compare.
        /// </param>
        public bool Equals(LibraryGenre x, LibraryGenre y)
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
        /// <param name="obj">The <see cref="LibraryGenre" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The type of <paramref name="obj" /> is a reference type and
        ///     <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(LibraryGenre obj)
        {
            return obj?.Name?.GetHashCode() ?? 0;
        }
    }
}