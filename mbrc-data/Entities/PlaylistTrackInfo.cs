namespace MusicBeeRemoteData.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The info of a <see cref="PlaylistTrack" />.
    ///     The info are stored seperately to avoid duplication since a track can appear to multiple playlists
    /// </summary>
    [DataContract]
    public class PlaylistTrackInfo : TypeBase, IEquatable<PlaylistTrackInfo>, IComparable<PlaylistTrackInfo>
    {
        /// <summary>
        ///     The artist performing the track.
        /// </summary>
        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        /// <summary>
        ///     The path of the track in the filesystem.
        /// </summary>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        /// <summary>
        ///     Used internally for sorting
        /// </summary>
        [IgnoreDataMember]
        public int Position { get; set; }

        /// <summary>
        ///     The title of the track.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        ///     Compares two tracks to figure the order. The tracks are always sorted by the <see cref="Position" />.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PlaylistTrackInfo other)
        {
            return this.Position.CompareTo(other.Position);
        }

        /// <summary>
        ///     Checks if an <paramref name="other" /> track is equal to this track. For
        ///     two tracks to be equal their Path properties must be equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PlaylistTrackInfo other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return ReferenceEquals(this, other) || this.Path.Equals(other.Path);
        }
    }
}