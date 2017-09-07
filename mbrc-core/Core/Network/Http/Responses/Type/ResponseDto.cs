using System.Runtime.Serialization;
using MusicBeeRemote.Core.Feature.Player;
using MusicBeeRemote.Core.Network.Http.Api;

namespace MusicBeeRemote.Core.Network.Http.Responses.Type
{
    [DataContract]
    public class ApiResponse
    {
        [DataMember(Name = "code")]
        public int Code { get; set; } = ApiCodes.Success;
    }

    [DataContract]
    public class ErrorResponse : ApiResponse
    {
        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }

    [DataContract]
    public class LyricsResponse : ApiResponse
    {
        [DataMember(Name = "lyrics")]
        public string Lyrics { get; set; }
    }

    [DataContract]
    public class RatingResponse : ApiResponse
    {
        [DataMember(Name = "rating")]
        public float Rating { get; set; }
    }

    [DataContract]
    public class PositionResponse : ApiResponse
    {
        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }
    }

    [DataContract]
    public class LfmRatingResponse : ApiResponse
    {
        [DataMember(Name = "status")]
        public LastfmStatus Status { get; set; }
    }
}