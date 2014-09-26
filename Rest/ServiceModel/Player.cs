#region

using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Type;
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
    public class GetScrobbleStatus : IReturn<StatusResponse>
    {
    }

    [Route("/player/scrobble", "PUT")]
    public class SetScrobbleStatus : IReturn<SuccessResponse>
    {
        public bool enabled { get; set; }
    }

    [Route("/player/repeat", "GET")]
    public class GetRepeatMode : IReturn<ValueResponse>
    {
    }

    [Route("/player/repeat", "PUT")]
    public class SetRepeatMode : IReturn<SuccessResponse>
    {
        public string mode { get; set; }
    }

    [Route("/player/mute", "GET")]
    public class GetMuteStatus : IReturn<StatusResponse>
    {
    }

    [Route("/player/mute", "PUT")]
    public class SetMuteStatus : IReturn<SuccessResponse>
    {
        public bool enabled { get; set; }
    }

    [Route("/player/volume", "GET")]
    public class GetVolume : IReturn<VolumeResponse>
    {
    }

    [Route("/player/volume", "PUT")]
    public class SetVolume : IReturn<SuccessResponse>
    {
        public int value { get; set; }
    }

    [Route("/player/autodj", "GET")]
    public class GetAutoDjStatus : IReturn<StatusResponse>
    {
    }

    [Route("/player/autodj", "PUT")]
    public class SetAutoDjStatus : IReturn<SuccessResponse>
    {
        public bool enabled { get; set; }
    }

    [Route("/player/previous", "GET")]
    public class PlayPrevious : IReturn<SuccessResponse>
    {
    }

    [Route("/player/next", "GET")]
    public class PlayNext : IReturn<SuccessResponse>
    {
    }

    [Route("/player/play", "GET")]
    public class PlaybackStart : IReturn<SuccessResponse>
    {
    }

    [Route("/player/stop", "GET")]
    public class PlaybackStop : IReturn<SuccessResponse>
    {
    }

    [Route("/player/pause", "GET")]
    public class PlaybackPause : IReturn<SuccessResponse>
    {
    }

    [Route("/player/status", "GET")]
    public class GetPlayerStatus : IReturn<PlayerStatus>
    {
    }

    [Route("/player/playstate", "GET")]
    public class GetPlayState : IReturn<ValueResponse>
    {
    }

    [DataContract]
    public class StatusResponse
    {
        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }

    [DataContract]
    public class VolumeResponse
    {
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }

    [DataContract]
    public class ValueResponse
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}