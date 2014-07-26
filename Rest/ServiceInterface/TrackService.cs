using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;

namespace MusicBeePlugin.Rest.ServiceInterface
{
    class TrackService : Service
    {
        public Track Get(GetTrack request)
        {
            return Plugin.Instance.TrackModule.GetTrackInfo();
        }

        public TrackCoverResponse Get(GetTrackCover request)
        {
            return new TrackCoverResponse()
            {
                cover = LyricCoverModel.Instance.Cover
            };
        }

        public TrackLyricsResponse Get(GetTrackLyrics request)
        {
            return new TrackLyricsResponse()
            {
                lyrics = LyricCoverModel.Instance.Lyrics
            };
        }

        public TrackRatingResponse Get(GetTrackRating request)
        {
            return new TrackRatingResponse()
            {
                rating = Plugin.Instance.TrackModule.GetRating()
            };
        }

        public TrackRatingResponse Put(SetTrackRating request)
        {
            return new TrackRatingResponse()
            {
                rating = Plugin.Instance.TrackModule.SetRating(request.rating)
            };
        }

        public TrackPositionResponse Get(GetTrackPosition request)
        {
            return Plugin.Instance.TrackModule.GetPosition();
        }

        public TrackPositionResponse Put(SetTrackPosition request)
        {
            return Plugin.Instance.TrackModule.SetPosition(request.position);
        }
    }
}
