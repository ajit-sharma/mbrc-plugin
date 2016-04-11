namespace MusicBeeRemoteData.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     A PlaylistTrack is a many to many relationship between
    ///     <see cref="PlaylistTrackInfo" /> and <see cref="Playlist" />.
    /// </summary>
    [DataContract]
    public class PlaylistTrack : TypeBase, IEquatable<PlaylistTrack>
    {
        /// <summary>
        ///     The id of the playlist in which the entry exists.
        /// </summary>
        [DataMember(Name = "playlist_id")]
        public long PlaylistId { get; set; }

        /// <summary>
        ///     The position of the track in the playlist.
        /// </summary>
        [DataMember(Name = "position")]
        public int Position { get; set; }

        /// <summary>
        ///     The id of the track info related with the current entry.
        /// </summary>
        [DataMember(Name = "track_info_id")]
        public long TrackInfoId { get; set; }

        /// <summary>
        ///     Checks if two <see cref="PlaylistTrack" />s are equal.
        ///     The TrackInfoId, PlaylistId and position must be equal for two
        ///     PlaylistTrack entries to be equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PlaylistTrack other)
        {
            return this.TrackInfoId == other.TrackInfoId && this.PlaylistId == other.PlaylistId
                   && this.Position == other.Position;
        }

        public override string ToString()
        {
            return $"{Position}: id: {Id} track {TrackInfoId}";
        }
    }
}