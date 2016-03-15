namespace MusicBeeRemoteCore.AndroidRemote.Entities
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [DataContract]
    public class NotificationMessage
    {
        public const string AutoDjStarted = "autodj-started";

        public const string AutoDjStopped = "autodj-stopped";

        public const string CoverChanged = "cover-changed";

        public const string LyricsChanged = "lyrics-changed";

        public const string MuteStatusChanged = "mute-status-changed";

        public const string NowPlayingListChanged = "nowplaying-list-changed";

        public const string PlayStatusChanged = "play-status-changed";

        public const string PositionChanged = "position-changed";

        public const string RatingChanged = "rating-changed";

        public const string RepeatStatusChanged = "repeat-status-changed";

        public const string ScrobbleStatusChanged = "scrobble-status-changed";

        public const string ShuffleStatusChanged = "shuffle-status-changed";

        public const string TrackChanged = "track-changed";

        public const string VolumeChanged = "volume-changed";

        public NotificationMessage(string message)
        {
            this.Message = message;
        }

        public NotificationMessage(JObject obj)
        {
            this.Message = (string)obj["message"];
        }

        public NotificationMessage()
        {
        }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public string ToJsonString()
        {
            var serialized = JsonConvert.SerializeObject(this, Formatting.None);
            return $"{serialized}\r\n";
        }
    }
}