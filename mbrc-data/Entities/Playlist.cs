﻿using System;
using System.Linq;
using System.Runtime.Serialization;

namespace MusicBeeRemoteData.Entities
{
    /// <summary>
    ///     Represent a Playlist stored in the database.
    /// </summary>
    [DataContract]
    public class Playlist : TypeBase, IComparable<Playlist>, IEquatable<Playlist>
    {
        /// <summary>
        ///     The <c>Playlists</c> automatically generated by MusicBee.
        ///     These are read only.
        /// </summary>
        private readonly string[] _readOnlyPlaylists =
            {
                "Recently Added", "Recently Played", "Top 25 Most Played", 
                "Top Rated"
            };

        /// <summary>
        ///     Backing field of <see cref="Name" /> property.
        /// </summary>
        private string _name;

        /// <summary>
        ///     The name of the <see cref="Playlist" />.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get => _name;

            set
            {
                _name = value;
                ReadOnly = _readOnlyPlaylists.Contains(_name);
            }
        }

        /// <summary>
        ///     The full path of the <c>playlist</c> in the filesystem.
        /// </summary>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        /// <summary>
        ///     A read only <see cref="Playlist" /> is one automatically generated by MusicBee.
        ///     Check <see cref="_readOnlyPlaylists" />.
        /// </summary>
        [DataMember(Name = "read_only")]
        public bool ReadOnly { get; private set; }

        /// <summary>
        ///     The number of the tracks contained in the <see cref="Playlist" />.
        /// </summary>
        [DataMember(Name = "tracks")]
        public int Tracks { get; set; }

        /// <summary>
        ///     Compares two <c>playlists</c>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Playlist other)
        {
            return string.Compare(Path, other.Path, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Checks if two <c>playlists</c> are equal.
        ///     Two play lists are equal if they have the same <see cref="Path" /> property.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Playlist other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Path.Equals(other.Path);
        }
    }
}