namespace MusicBeePlugin.Rest.ServiceModel.Type
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     Represent a track in the now playing list.
    ///     The tracks in the now playlist may or may not be in the Library.
    /// </summary>
    [DataContract]
    public class NowPlaying : TypeBase
    {
        /// <summary>
        ///     The artist of the track.
        /// </summary>
        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        /// <summary>
        ///     The full path of the track in the file system.
        /// </summary>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        /// <summary>
        ///     The position of the track in the now playing list.
        /// </summary>
        [DataMember(Name = "position")]
        public int Position { get; set; }

        /// <summary>
        ///     The title of the track.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}