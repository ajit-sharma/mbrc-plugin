#region

using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route("/nowplaying/", "GET", Summary = "Retrieves the tracks in the now playing list.")]
    public class AllNowPlaying : IReturn<PaginatedNowPlayingResponse>
    {
        [ApiMember(Name = "limit", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The number of results contained in the response.")]
        public int offset { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The position of the first entry in the response.")]
        public int limit { get; set; }
    }

    [Route("/nowplaying/play", "PUT", Summary = "Plays the track specified.")]
    public class NowPlayingPlay : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "path", ParameterType = "path", DataType = "string", IsRequired = true,
            Description = "The full path of the track in the filesystem.")]
        public string path { get; set; }
    }

    [Route("/nowplaying/{id}", "DELETE", Summary = "Removes a track from the now playing list.")]
    public class NowPlayingRemove : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "id", ParameterType = "path", DataType = "integer", IsRequired = true,
            Description = "The id of the track to be removed from the now playing list.")]
        public int id { get; set; }
    }

    [Route("/nowplaying/move", "PUT", Summary = "Moves a track from a position in the now playing list to another.")]
    public class NowPlayingMove : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "from", ParameterType = "query", DataType = "integer", IsRequired = true,
            Description = "The initial position of the track we want to move in the now playing list.")]
        public int from { get; set; }

        [ApiMember(Name = "to", ParameterType = "query", DataType = "integer", IsRequired = true,
            Description = "The new position where the element will be moved.")]
        public int to { get; set; }
    }

    [Route("/nowplaying/queue", "PUT", Summary = "Queues item for playing in the now playing queue.",
        Notes = "The item can be either a single track by id, or the tracks belonging to a genre," +
                " artist or album.")]
    public class NowPlayingQueue : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "type", ParameterType = "query", DataType = "string", IsRequired = true,
            Description = "The type of meta data the id responds to. Helps identify in which table to lookup.")]
        [ApiAllowableValues("type", typeof (MetaTag))]
        public MetaTag type { get; set; }

        [ApiMember(Name = "action", ParameterType = "query", DataType = "string", IsRequired = true,
            Description = "The action should be 'now' for clearing queue and immediately " +
                          "playing the tracks matching, 'next' for queuing after the current track" +
                          ", or 'last' for queue last")]
        [ApiAllowableValues("action", typeof (QueueType))]
        public QueueType action { get; set; }

        [ApiMember(Name = "id", ParameterType = "query", DataType = "long`", IsRequired = true,
            Description = "The id of the entire entity for which we want to queue tracks.")]
        public long id { get; set; }
    }
}