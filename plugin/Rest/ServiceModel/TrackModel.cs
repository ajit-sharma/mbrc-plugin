#region Dependencies

using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
	[Api("The API responsible for the track related operations")]
	[Route(Routes.Track, Verbs.Get, Summary = Summary.TrackGet)]
	public class GetTrack : IReturn<Track>
	{
	}

	[Api]
	[Route(Routes.TrackRating, Verbs.Get, Summary = Summary.RatingGet)]
	public class GetTrackRating : IReturn<TrackRatingResponse>
	{
	}

	[Api]
	[Route(Routes.TrackRating, Verbs.Post, Summary = Summary.RatingPut)]
	public class SetTrackRating : IReturn<TrackRatingResponse>
	{
		[ApiMember(Name = "rating", ParameterType = "body", DataType = SwaggerType.Float, IsRequired = false,
			Description = Description.Rating)]
		[ApiAllowableValues("rating", 0, 5)]
		public float? Rating { get; set; }
	}

	[Api]
	[Route(Routes.TrackPosition, Verbs.Get, Summary = Summary.TrackPositionGet)]
	public class GetTrackPosition : IReturn<TrackPositionResponse>
	{
	}

	[Api]
	[Route(Routes.TrackPosition, Verbs.Post, Summary = Summary.TrackPositionSet)]
	public class SetTrackPosition : IReturn<TrackPositionResponse>
	{
		[ApiMember(Name = "position", ParameterType = "body", DataType = SwaggerType.Int, IsRequired = true,
			Description = Description.Position)]
		public int Position { get; set; }
	}

	[Api]
	[Route(Routes.TrackCover, Verbs.Get, Summary = Summary.Cover)]
	public class GetTrackCoverData
	{
		[ApiMember(Name = "size", IsRequired = false, ParameterType = "query", DataType = SwaggerType.Int,
			Description = Description.CoverSize)]
		public int? Size { get; set; }
	}

	[Api]
	[Route(Routes.TrackLyrics, Verbs.Get, Summary = Summary.LyricsGet)]
	public class GetTrackLyrics : IReturn<TrackLyricsResponse>
	{
	}

	[Api]
	[Route(Routes.TrackLyricsRaw, Verbs.Get, Summary = Summary.LyricsRaw)]
	public class GetTrackLyricsText : IReturn<string>
	{
	}
}