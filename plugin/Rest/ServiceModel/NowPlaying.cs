#region

using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
	[Api("The API responsible for the now playing list operations")]
    [Route(Routes.Nowplaying, Verbs.Get, Summary = Summary.NowPlayingGet)]
    public class AllNowPlaying : IReturn<PaginatedNowPlayingResponse>
    {
		[ApiMember(Name = "offset", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false,
            Description = Description.Offset)]
        public int Offset { get; set; }

        [ApiMember(Name = "limit", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false,
            Description = Description.Limit)]
        public int Limit { get; set; }
    }

	[Api]
    [Route(Routes.NowplayingPlay, Verbs.Put, Summary = Summary.NowPlayingPlay)]
    public class NowPlayingPlay : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "path", ParameterType = "query", DataType = SwaggerType.String, IsRequired = true,
            Description = Description.NowPlayingPath)]
        public string Path { get; set; }
    }

	[Api]
	[Route(Routes.NowplayingId, Verbs.Delete, Summary = Summary.NowPlayingDelete)]
    public class NowPlayingRemove : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true,
            Description = Description.NowPlayingId)]
        public int Id { get; set; }
    }

	[Api]
	[Route(Routes.NowplayingMove, Verbs.Put, Summary = Summary.NowPlayingMove)]
    public class NowPlayingMove : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "from", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = true,
            Description = Description.NowPlayingFrom)]
        public int From { get; set; }

        [ApiMember(Name = "to", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = true,
            Description = Description.NowPlayingTo)]
        public int To { get; set; }
    }

	[Api]
	[Route(Routes.NowplayingQueue, Verbs.Put, Summary = Summary.NowPlayingQueue, Notes = Notes.NowPlayingQueue)]
    public class NowPlayingQueue : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "type", ParameterType = "query", DataType = SwaggerType.String, IsRequired = true, Description = Description.MetaType)]
        [ApiAllowableValues("type", typeof (MetaTag))]
        public MetaTag Type { get; set; }

        [ApiMember(Name = "action", ParameterType = "query", DataType = SwaggerType.String, IsRequired = true, Description = Description.MoveAction)]
        [ApiAllowableValues("action", typeof (QueueType))]
        public QueueType Action { get; set; }

        [ApiMember(Name = "id", ParameterType = "query", DataType = SwaggerType.Long, IsRequired = true, Description = Description.NowPlayingQueueId)]
        public long Id { get; set; }
    }
}