using System.IO;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

namespace MusicBeePlugin.Rest.ServiceModel
{

    [Route("/track", "GET")]
    public class GetTrack : IReturn<Track>
    {

    }

    [Route("/track/rating", "GET")]
    public class GetTrackRating : IReturn<TrackRatingResponse>
    {

    }

    [Route("/track/rating", "PUT")]
    public class SetTrackRating : IReturn<TrackRatingResponse>
    {
        public int rating { get; set; }
    }

    
    [Route("/track/position", "GET")]
    public class GetTrackPosition : IReturn<TrackPositionResponse>
    {
        
    }

    [Route("/track/position", "PUT")]
    public class SetTrackPosition : IReturn<TrackPositionResponse>
    {
        public int position { get; set; }
    }

    [Route("/track/cover", "GET")]
    public class GetTrackCover : IReturn<TrackCoverResponse>
    {
        public int? size { get; set; }
    }

    [Route("/track/cover/raw")]
    public class GetTrackCoverData : IReturn<Stream>
    {        
    }

    [Route("/track/lyrics", "GET")]
    public class GetTrackLyrics : IReturn<TrackLyricsResponse>
    {
    }

    [Route("/track/lyrics/raw", "GET")]
    public class GetTrackLyricsText : IReturn<string>
    {
        
    }

    public class TrackLyricsResponse
    {
        public string lyrics { get; set; }
    }

    public class TrackRatingResponse
    {
        public float rating { get; set; }
    }


    public class TrackPositionResponse
    {
        public int position { get; set; }
        public int duration { get; set; }
    }

    public class TrackCoverResponse
    {
        public string cover { get; set; }
    }
}
