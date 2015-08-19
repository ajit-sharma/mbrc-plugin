#region

using System.Runtime.Serialization;
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
    [DataContract]
    public class NowPlayingPlay : IReturn<SuccessResponse>
    {
		[DataMember(Name = "path")]
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

	[Route(Routes.NowplayingMove, Verbs.Put, Summary = Summary.NowPlayingMove)]
    [DataContract]
    public class NowPlayingMove : IReturn<SuccessResponse>
    {
        [DataMember(Name = "from", IsRequired = true)]
        public int From { get; set; }

        [DataMember(Name = "to", IsRequired = true)]
        public int To { get; set; }
    }

	[Api]
	[Route(Routes.NowplayingQueue, Verbs.Put, Summary = Summary.NowPlayingQueue, Notes = Notes.NowPlayingQueue)]
    [DataContract]
    public class NowPlayingQueue : IReturn<SuccessResponse>
    {
        [DataMember(Name = "type")]
        public MetaTag Type { get; set; }

        [DataMember(Name ="action")]
        public QueueType Action { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }
    }
}