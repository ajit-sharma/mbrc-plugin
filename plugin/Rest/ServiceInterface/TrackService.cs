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

        public TrackInfoResponse Get(GetTrack request)
        {
            return _module.GetTrackInfo();
        }

        public LyricsResponse Get(GetTrackLyrics request)
        {
            return new LyricsResponse
            {
                Lyrics = _model.Lyrics
            };
        }

        public RatingResponse Get(GetTrackRating request)
        {
            return new RatingResponse
            {
                Rating = _module.GetRating()
            };
        }

        public RatingResponse Put(SetTrackRating request)
        {
            return new RatingResponse
            {
                Rating = _module.SetRating(request.Rating ?? -1)
            };
        }

        public PositionResponse Get(GetTrackPosition request)
        {
            return _module.GetPosition();
        }

        public PositionResponse Put(SetTrackPosition request)
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