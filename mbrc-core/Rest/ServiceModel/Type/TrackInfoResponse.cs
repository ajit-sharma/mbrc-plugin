namespace MusicBeeRemoteCore.Rest.ServiceModel.Type
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the basic information of a track.
    /// Such as used in the display of the currently playing track.
    /// </summary>
    [DataContract]
    public class TrackInfoResponse : ResponseBase
    {
        /// <summary>
        /// The album the track is part of.
        /// </summary>
        [DataMember(Name = "album")]
        public string Album { get; set; }

        /// <summary>
        /// The name of the artist performing the track.
        /// </summary>
        [DataMember(Name = "artist")]
        public string Artist { get; set; }

        /// <summary>
        /// The path of the audio file in the filesystem.
        /// </summary>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        /// <summary>
        /// The title of the track.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The year of the albums release.
        /// </summary>
        [DataMember(Name = "year")]
        public string Year { get; set; }
    }
}