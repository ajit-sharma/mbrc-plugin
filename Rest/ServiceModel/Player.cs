#region

using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route("/player/shuffle", "GET")]
    public class GetShuffleState : IReturn<StatusResponse>
    {
    }

    [Route("/player/shuffle", "PUT")]
    public class SetShuffleState : IReturn<SuccessResponse>
    {
        public bool enabled { get; set; }
    }

    [Route("/player/scrobble", "GET")]
    public class GetScrobbleStatus
    {
    }

    [Route("/player/scrobble", "PUT")]
    public class SetScrobbleStatus
    {
        public bool enabled { get; set; }
    }

    [Route("/player/repeat", "GET")]
    public class GetRepeateMode
    {
    }

    [Route("/player/repeat", "PUT")]
    public class SetRepeateMode
    {
        public string mode { get; set; }
    }

    [Route("/player/mute", "GET")]
    public class GetMuteStatus
    {
    }

    [Route("/player/mute", "PUT")]
    public class SetMuteStatus
    {
        public bool enabled { get; set; }
    }

    [Route("/player/volume", "GET")]
    public class GetVolume
    {
    }

    [Route("/player/volume", "PUT")]
    public class SetVolume
    {
        public int value { get; set; }
    }

    [Route("/player/autodj", "GET")]
    public class GetAutoDjStatus
    {
    }

    [Route("/player/autodj", "PUT")]
    public class SetAutoDjStatus
    {
        public bool enabled { get; set; }
    }

    [Route("/player/previous", "GET")]
    public class PlayPrevious
    {
    }

    [Route("/player/next", "GET")]
    public class PlayNext
    {
    }

    [Route("/player/play", "GET")]
    public class PlaybackStart
    {
    }

    [Route("/player/stop", "GET")]
    public class PlaybackStop
    {
    }

    [Route("/player/pause", "GET")]
    public class PlaybackPause
    {
    }

    [DataContract]
    public class StatusResponse
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }
}