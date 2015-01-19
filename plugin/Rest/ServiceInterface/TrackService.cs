#region

using System.IO;
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

        public TrackCoverResponse Get(GetTrackCover request)
        {
            return new TrackCoverResponse
            {
                cover = _model.Cover
            };
        }

        public TrackLyricsResponse Get(GetTrackLyrics request)
        {
            return new TrackLyricsResponse
            {
                lyrics = _model.Lyrics
            };
        }

        public TrackRatingResponse Get(GetTrackRating request)
        {
            return new TrackRatingResponse
            {
                rating = _module.GetRating()
            };
        }

        public TrackRatingResponse Put(SetTrackRating request)
        {
            return new TrackRatingResponse
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