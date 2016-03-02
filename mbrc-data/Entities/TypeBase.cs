namespace MusicBeeRemoteData.Entities
{
    using System;
    using System.Runtime.Serialization;

    using MusicBeeRemoteData.Extensions;

    /// <summary>
    ///     The <c>base</c> of all the POCOs stored in the database.
    /// </summary>
    [DataContract(Name = "typeBase")]
    public class TypeBase
    {
        /// <summary>
        ///     The Date the entry was added. It defaults in the UTC <see cref="DateTime" />
        ///     of the <see langword="object" /> creation. Used during sync to determine
        ///     the newer additions.
        /// </summary>
        [DataMember(Name = "date_added")]
        public long DateAdded { get; set; } = DateTime.UtcNow.ToUnixTime();

        /// <summary>
        ///     The Date the entry was deleted. As it is expected for an entry that is
        ///     active <c>this</c> field should have a <see langword="null" /> value.
        ///     The objects are initially soft deleted and periodically a function is
        ///     responsible objects that have been deleted for a number of days.
        /// </summary>
        [DataMember(Name = "date_deleted")]
        public long DateDeleted { get; set; }

        /// <summary>
        ///     The Date the entry was last updated. It defaults in the UTC <see cref="DateTime" />
        ///     of the <see langword="object" /> creation. Used during sync to figure out the modified entries.
        /// </summary>
        [DataMember(Name = "date_updated")]
        public long DateUpdated { get; set; }

        /// <summary>
        ///     The identity of the entry stored in the database.
        /// </summary>
        [DataMember(Name = "id")]
        public long Id { get; set; }
    }
}