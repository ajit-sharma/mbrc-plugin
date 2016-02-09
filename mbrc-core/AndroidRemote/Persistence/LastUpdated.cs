namespace MusicBeePlugin.AndroidRemote.Persistence
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Keeps the dates of the most recent changes in the database.
    ///     Used for the sync functionality.
    /// </summary>
    [DataContract]
    public class LastUpdated
    {
        /// <summary>
        ///     The most recent update for the albums table.
        /// </summary>
        [DataMember]
        public DateTime AlbumsUpdated { get; set; }

        /// <summary>
        ///     The most recent update for the artists table.
        /// </summary>
        [DataMember]
        public DateTime ArtistsUpdated { get; set; }

        /// <summary>
        ///     All the entries before this Date have been purged from the Database.
        /// </summary>
        [DataMember]
        public DateTime DeleteThreshold { get; set; }

        /// <summary>
        ///     The most recent update for the genres table.
        /// </summary>
        [DataMember]
        public DateTime GenresUpdated { get; set; }

        /// <summary>
        ///     The id of the entry.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     The most recent update for the playlists table.
        /// </summary>
        [DataMember]
        public DateTime PlaylistsUpdated { get; set; }

        /// <summary>
        ///     The most recent update for the tracks table.
        /// </summary>
        [DataMember]
        public DateTime TracksUpdated { get; set; }
    }
}