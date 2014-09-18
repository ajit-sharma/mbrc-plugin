using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Ninject;
using ServiceStack.ServiceInterface;

namespace MusicBeePlugin.Rest.ServiceInterface
{
    class TrackService : Service
    {
        private readonly TrackModule _module;

        public TrackService()
        {
            var kernel = new StandardKernel(new InjectionModule());
            _module = kernel.Get<TrackModule>();
        }

        public Track Get(GetTrack request)
        {
            return _module.GetTrackInfo();
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
                rating = _module.GetRating()
            };
        }

        public TrackRatingResponse Put(SetTrackRating request)
        {
            return new TrackRatingResponse()
            {
                rating = _module.SetRating(request.rating)
            };
        }

        public TrackPositionResponse Get(GetTrackPosition request)
        {
            return _module.GetPosition();
        }

        public TrackPositionResponse Put(SetTrackPosition request)
        {
            return _module.SetPosition(request.position);
        }
    }
}
