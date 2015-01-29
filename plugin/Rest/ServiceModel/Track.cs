#region

using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route(Routes.Track, Verbs.Get, Summary = Summary.TrackGet)]
    public class GetTrack : IReturn<Track>
    {
    }

    [Route(Routes.TrackRating, Verbs.Get, Summary = Summary.RatingGet)]
    public class GetTrackRating : IReturn<TrackRatingResponse>
    {
    }

    [Route(Routes.TrackRating, Verbs.Put, Summary = Summary.RatingPut)]
    public class SetTrackRating : IReturn<TrackRatingResponse>
    {
        [ApiMember(Name = "rating", ParameterType = "query", DataType = "float", IsRequired = false,
            Description = Description.Rating)]
        [ApiAllowableValues("rating", 0, 5)]
        public float? Rating { get; set; }
    }


    [Route(Routes.TrackPosition, Verbs.Get, Summary = Summary.TrackPositionGet)]
    public class GetTrackPosition : IReturn<TrackPositionResponse>
    {
    }

    [Route(Routes.TrackPosition, Verbs.Put, Summary = Summary.TrackPositionSet)]
    public class SetTrackPosition : IReturn<TrackPositionResponse>
    {
        [ApiMember(Name = "position", ParameterType = "query", DataType = "integer", IsRequired = true,
            Description = Description.Position)]
        public int Position { get; set; }
    }

    [Route(Routes.TrackCover, Verbs.Get, Summary = Summary.Cover)]
    public class GetTrackCoverData
    {
        [ApiMember(Name = "size", IsRequired = false)]
        public int? Size { get; set; }
    }

    [Route(Routes.TrackLyrics, Verbs.Get, Summary = Summary.LyricsGet)]
    public class GetTrackLyrics : IReturn<TrackLyricsResponse>
    {
    }

    [Route(Routes.TrackLyricsRaw, Verbs.Get, Summary = Summary.LyricsRaw)]
    public class GetTrackLyricsText : IReturn<string>
    {
    }

    [DataContract]
    public class TrackLyricsResponse
    {
        [DataMember(Name = "lyrics")]
        public string Lyrics { get; set; }
    }

    [DataContract]
    public class TrackRatingResponse
    {
        [DataMember(Name = "rating")]
        public float Rating { get; set; }
    }

    [DataContract]
    public class TrackPositionResponse
    {
        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }
    }
}