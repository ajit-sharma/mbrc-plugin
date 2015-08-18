#region

using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class TrackService : Service
    {
        private readonly LyricCoverModel _model;
        private readonly TrackModule _module;

        public TrackService(TrackModule module, LyricCoverModel model)
        {
            _module = module;
            _model = model;
        }

        public Track Get(GetTrack request)
        {
            return _module.GetTrackInfo();
        }

        public TrackLyricsResponse Get(GetTrackLyrics request)
        {
            return new TrackLyricsResponse
            {
                Lyrics = _model.Lyrics
            };
        }

        public TrackRatingResponse Get(GetTrackRating request)
        {
            return new TrackRatingResponse
            {
                Rating = _module.GetRating()
            };
        }

        public TrackRatingResponse Post(SetTrackRating request)
        {
            return new TrackRatingResponse
            {
                Rating = _module.SetRating(request.Rating ?? -1)
            };
        }

        public TrackPositionResponse Get(GetTrackPosition request)
        {
            return _module.GetPosition();
        }

        public TrackPositionResponse Post(SetTrackPosition request)
        {
            return _module.SetPosition(request.Position);
        }

        [AddHeader(ContentType = "image/jpeg")]
        public object Get(GetTrackCoverData request)
        {
            return new HttpResult(_module.GetBinaryCoverData(), "image/jpeg");
        }

        [AddHeader(ContentType = "text/plain")]
        public string Get(GetTrackLyricsText request)
        {
            return _model.Lyrics;
        }
    }
}