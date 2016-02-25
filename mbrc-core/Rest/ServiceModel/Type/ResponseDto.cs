namespace MusicBeeRemoteCore.Rest.ServiceModel.Type
{
    using System.Runtime.Serialization;

    using MusicBeeRemoteCore.AndroidRemote.Enumerations;
    using MusicBeeRemoteCore.Rest.ServiceInterface;

    [DataContract]
    public class ResponseBase
    {
        [DataMember(Name = "code")]
        public int Code { get; set; } = ApiCodes.Success;
    }

    [DataContract]
    public class LyricsResponse : ResponseBase
    {
        [DataMember(Name = "lyrics")]
        public string Lyrics { get; set; }
    }

    [DataContract]
    public class RatingResponse : ResponseBase
    {
        [DataMember(Name = "rating")]
        public float Rating { get; set; }
    }

    [DataContract]
    public class PositionResponse : ResponseBase
    {
        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }
    }

    [DataContract]
    public class LfmRatingResponse : ResponseBase
    {
        [DataMember(Name = "status")]
        public LastfmStatus Status { get; set; }
    }
}