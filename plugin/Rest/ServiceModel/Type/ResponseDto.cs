using System.Runtime.Serialization;

namespace MusicBeePlugin.Rest.ServiceModel.Type
{
	[DataContract]
	public class SuccessResponse
	{
		[DataMember(Name = "success")]
		public bool Success { get; set; }
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
