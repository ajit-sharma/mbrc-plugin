#region

using System.Text;
using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Nancy;
using Nancy.ModelBinding;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class TrackApiModule : NancyModule
    {
        private readonly LyricCoverModel _model;
        private readonly TrackModule _module;

        public TrackApiModule(TrackModule module, LyricCoverModel model)
        {
            _module = module;
            _model = model;

            Get["/track"] = _ => _module.GetTrackInfo();
            Get["/track/lyrics"] = _ => new LyricsResponse
            {
                Lyrics = _model.Lyrics
            };

            Get["/track/rating"] = _ => new RatingResponse
            {
                Rating = _module.GetRating()
            };

            Put["/track/rating"] = _ =>
            {
                var request = this.Bind<SetTrackRating>();
                return new RatingResponse
                {
                    Rating = _module.SetRating(request.Rating ?? -1)
                };
            };

            Get["/track/position"] = _ => _module.GetPosition();

            Put["/track/position"] = _ =>
            {
                var request = this.Bind<SetTrackPosition>();
                return _module.SetPosition(request.Position);
            };

            Get["/track/lfmrating"] = _ => new LfmRatingResponse
            {
                Status = _module.RequestLoveStatus(string.Empty),
                Code = ApiCodes.Success
            };

            Put["/track/lfmrating"] = _ =>
            {
                var request = this.Bind<PutLfmRating>();
                return new LfmRatingResponse
                {
                    Status = _module.RequestLoveStatus(request.Status),
                    Code = ApiCodes.Success
                };
            };

            Get["/track/cover"] = _ =>
            {
                return new Response
                {
                    ContentType = "image/jpeg",
                    Contents = stream => _module.GetBinaryCoverData()
                };
            };

            Get["/track/lyrics/raw"] = _ =>
            {
                return new Response
                {
                    ContentType = "text/plain",
                    Contents = stream =>
                    {
                        var data = Encoding.UTF8.GetBytes(_model.Lyrics);
                        stream.Write(data, 0, data.Length);
                    }
                };
            };
        }
    }
}