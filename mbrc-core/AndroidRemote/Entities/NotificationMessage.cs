namespace MusicBeePlugin.AndroidRemote.Entities
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [DataContract]
    public class NotificationMessage
    {
        public const string AutoDjStarted = "autodj-started";

        public const string AutoDjStopped = "autodj-stopped";

        public const string CoverChanged = "cover-Changed";

        public const string LyricsChanged = "lyrics-Changed";

        public const string MuteStatusChanged = "mute-status-Changed";

        public const string NowPlayingListChanged = "nowplaying-list-Changed";

        public const string PlayStatusChanged = "play-status-Changed";

        public const string PositionChanged = "position-Changed";

        public const string RatingChanged = "rating-Changed";

        public const string RepeatStatusChanged = "repeat-status-Changed";

        public const string ScrobbleStatusChanged = "scrobble-status-Changed";

        public const string ShuffleStatusChanged = "shuffle-status-Changed";

        public const string TrackChanged = "track-Changed";

        public const string VolumeChanged = "volume-Changed";

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