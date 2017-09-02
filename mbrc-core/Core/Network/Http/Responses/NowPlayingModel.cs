using System.Runtime.Serialization;
using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Feature.NowPlaying;

namespace MusicBeeRemote.Core.Network.Http.Responses
{
    [DataContract]
    public class NowPlayingPlay
    {
        [DataMember(Name = "path")]
        public string Path { get; set; }
    }

    [DataContract]
    public class NowPlayingRemove
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
    }

    [DataContract]
    public class NowPlayingMove
    {
        [DataMember(Name = "from", IsRequired = true)]
        public int From { get; set; }

        [DataMember(Name = "to", IsRequired = true)]
        public int To { get; set; }
    }

    [DataContract]
    public class NowPlayingQueue
    {
        [DataMember(Name = "action")]
        public QueueType Action { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "type")]
        public MetaTag Type { get; set; }
    }
}