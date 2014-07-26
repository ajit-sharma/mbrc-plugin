using System.Collections.Generic;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route("/nowplaying/", "GET")]
    public class AllNowPlaying : IReturn<List<NowPlaying>>
    {
        public int offset { get; set; }
        public int limit { get; set; }
    }

    [Route("/nowplaying/play", "PATCH")]
    public class NowPlayingPlay : IReturn<NowPlayingSuccessResponse>
    {
        public string path { get; set; }
    }

    [Route("/nowplaying/{id}", "DELETE")]
    public class NowPlayingRemove : IReturn<NowPlayingSuccessResponse>
    {
        public int id { get; set; }
    }

    [Route("/nowplaying/{id}", "PATCH")]
    public class NowPlayingMove : IReturn<object>
    {
        public int id { get; set; }
        public int moveto { get; set; }
    }

    public class NowPlayingSuccessResponse
    {
        public bool success { get; set; }
    }
}
