using System.Runtime.Serialization;

namespace MusicBeeRemote.Data.Entities
{
    /// <summary>
    ///     A cover entry stored in the database.
    /// </summary>
    [DataContract]
    public class LibraryCover : TypeBase
    {
        /// <summary>
        ///     A hash (sha1) used as a unique identifier for the cover.
        ///     It is also used as a filename for the cover binary file
        ///     in the filesystem.
        /// </summary>
        [DataMember(Name = "hash")]
        public string Hash { get; set; }
    }
}