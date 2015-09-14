#region

using ServiceStack.Text;
using System.Runtime.Serialization;

#endregion

namespace MusicBeePlugin.AndroidRemote.Entities
{
    [DataContract]
    public class NotificationMessage
    {
        public const string NowPlayingListChanged = "nowplaying-list-changed";
        public const string AutoDjStopped = "autodj-stopped";
        public const string AutoDjStarted = "autodj-started";
        public const string ScrobbleStatusChanged = "scrobble-status-changed";
        public const string MuteStatusChanged = "mute-status-changed";
        public const string PositionChanged = "position-changed";
        public const string TrackChanged = "track-changed";
        public const string PlayStatusChanged = "play-status-changed";
        public const string VolumeChanged = "volume-changed";
        public const string RepeatStatusChanged = "repeat-status-changed";
        public const string ShuffleStatusChanged = "shuffle-status-changed";
        public const string CoverChanged = "cover-changed";
        public const string LyricsChanged = "lyrics-changed";
        public const string RatingChanged = "rating-changed";

        public NotificationMessage(string message)
        {
            Message = message;
        }

        public NotificationMessage(JsonObject jObj)
        {
            Message = jObj.Get("message");
        }

        public NotificationMessage()
        {
        }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        public string ToJsonString()
        {
            var serialized = JsonSerializer.SerializeToString(this);
            return string.Format("{0}\r\n", serialized);
        }
    }
}