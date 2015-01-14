#region

using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route("/nowplaying/", "GET")]
    public class AllNowPlaying : IReturn<PaginatedResponse>
    {
        public int offset { get; set; }
        public int limit { get; set; }
    }

    [Route("/nowplaying/play", "PUT")]
    public class NowPlayingPlay : IReturn<SuccessResponse>
    {
        public string path { get; set; }
    }

    [Route("/nowplaying/{id}", "DELETE")]
    public class NowPlayingRemove : IReturn<SuccessResponse>
    {
        public int id { get; set; }
    }

    [Route("/nowplaying/move", "PUT")]
    public class NowPlayingMove : IReturn<SuccessResponse>
    {
        public int from { get; set; }
        public int to { get; set; }
    }

    [Route("/nowplaying/queue", "PUT")]
    public class NowPlayingQueue : IReturn<SuccessResponse>
    {
        public MetaTag type { get; set; }
        public QueueType action { get; set; }
        public long id { get; set; }
    }
}