#region Dependencies

using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Api("The API responsible for the track related operations")]
    [Route(Routes.Track, Verbs.Get, Summary = Summary.TrackGet)]
    public class GetTrack : IReturn<TrackInfoResponse>
    {
    }

    [Api]
    [Route(Routes.TrackRating, Verbs.Get, Summary = Summary.RatingGet)]
    public class GetTrackRating : IReturn<RatingResponse>
    {
    }

    [Api]
    [Route(Routes.TrackRating, Verbs.Put, Summary = Summary.RatingPut)]
    [DataContract]
    public class SetTrackRating : IReturn<RatingResponse>
    {
        [DataMember(Name = "rating", IsRequired = true)]
        [ApiAllowableValues("rating", 0, 5)]
        public float? Rating { get; set; }
    }

    [Api]
    [Route(Routes.TrackPosition, Verbs.Get, Summary = Summary.TrackPositionGet)]
    public class GetTrackPosition : IReturn<PositionResponse>
    {
    }

    [Api]
    [Route(Routes.TrackPosition, Verbs.Put, Summary = Summary.TrackPositionSet)]
    [DataContract]
    public class SetTrackPosition : IReturn<PositionResponse>
    {
        [DataMember(Name = "position", IsRequired = true)]
        public int Position { get; set; }
    }

    [Api]
    [Route(Routes.TrackCover, Verbs.Get, Summary = Summary.Cover)]
    public class GetTrackCoverData
    {
        [ApiMember(Name = "size", IsRequired = false, ParameterType = "query", DataType = SwaggerType.Int,
            Description = Descriptions.CoverSize)]
        public int? Size { get; set; }
    }

    [Api]
    [Route(Routes.TrackLyrics, Verbs.Get, Summary = Summary.LyricsGet)]
    public class GetTrackLyrics : IReturn<LyricsResponse>
    {
    }

    [Api]
    [Route(Routes.TrackLyricsRaw, Verbs.Get, Summary = Summary.LyricsRaw)]
    public class GetTrackLyricsText : IReturn<string>
    {
    }

    [Route(Routes.LfmRating, Verbs.Get, Summary = Summary.LfmRatingGet)]
    public class GetLfmRating : IReturn<LfmRatingResponse>
    {
    }

    [Route(Routes.LfmRating, Verbs.Put, Summary = Summary.LfmRatingPut)]
    public class PutLfmRating : IReturn<LfmRatingResponse>
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}