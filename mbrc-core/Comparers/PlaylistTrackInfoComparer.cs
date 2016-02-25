namespace MusicBeeRemoteCore.Comparers
{
    using System.Collections.Generic;

    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using MusicBeeRemoteData.Entities;

    /// <summary>
    /// Custom <c>EqualityComparer</c> for playlist tracks. 
    /// Since adding mutable properties in overridden GetHashCode is not
    /// recommended. 
    /// </summary>
    public class PlaylistTrackInfoComparer : IEqualityComparer<PlaylistTrackInfo>
    {
        /// <summary>
        /// With the property set <see langword="true"/> (default value) the comparer
        /// will include the Position property when comparing two elements.
        /// </summary>
        public bool IncludePosition { get; set; } = true;

        /// <summary>
        /// Checks the equality between two <see cref="PlaylistTrackInfo"/> objects.
        /// This IEqualityComparer regards two items Equal if the have the same path in
        /// the file system (and the same position in the playlist if the <see cref="IncludePosition"/>
        /// property is set to true.).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(PlaylistTrackInfo x, PlaylistTrackInfo y)
        {
            if (x == null && y == null || ReferenceEquals(x, null))
            {
                return false;
            }

            return ReferenceEquals(x, y) || this.IncludePosition
                       ? x.Path.Equals(y.Path) && x.Position.Equals(y.Position)
                       : x.Path.Equals(y.Path);
        }

        /// <summary>
        /// Gets the hash code for a <see cref="PlaylistTrackInfo"/> object. 
        /// The Object hash code is based on the Path property hash code,
        /// along with the position of the track in the playlist.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(PlaylistTrackInfo obj)
        {
            return (this.IncludePosition
                        ? obj?.Path?.GetHashCode() ^ obj?.Position.GetHashCode()
                        : obj?.Path?.GetHashCode()) ?? 0;
        }
    }
}