namespace MusicBeePlugin.Rest.ServiceInterface
{
    using System.Text;

    using MusicBeePlugin.AndroidRemote.Model;
    using MusicBeePlugin.Modules;
    using MusicBeePlugin.Rest.ServiceModel;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    public class TrackApiModule : NancyModule
    {
        private readonly LyricCoverModel _model;

        private readonly TrackModule _module;

        public TrackApiModule(TrackModule module, LyricCoverModel model)
        {
            this._module = module;
            this._model = model;

            this.Get["/track"] = _ => this.Response.AsJson(this._module.GetTrackInfo());
            this.Get["/track/lyrics"] = _ => new LyricsResponse { Lyrics = this._model.Lyrics };

            this.Get["/track/rating"] = _ => new RatingResponse { Rating = this._module.GetRating() };

            this.Put["/track/rating"] = _ =>
                {
                    var request = this.Bind<SetTrackRating>();
                    return new RatingResponse { Rating = this._module.SetRating(request.Rating ?? -1) };
                };

            this.Get["/track/position"] = _ => this._module.GetPosition();

            this.Put["/track/position"] = _ =>
                {
                    var request = this.Bind<SetTrackPosition>();
                    return this._module.SetPosition(request.Position);
                };

            this.Get["/track/lfmrating"] =
                _ =>
                new LfmRatingResponse
                    {
                        Status = this._module.RequestLoveStatus(string.Empty),
                        Code = ApiCodes.Success
                    };

            this.Put["/track/lfmrating"] = _ =>
                {
                    var request = this.Bind<PutLfmRating>();
                    return new LfmRatingResponse
                               {
                                   Status = this._module.RequestLoveStatus(request.Status), 
                                   Code = ApiCodes.Success
                               };
                };

            this.Get["/track/cover"] =
                _ =>
                    {
                        return new Response
                                   {
                                       ContentType = "image/jpeg", 
                                       Contents = stream => this._module.GetBinaryCoverData()
                                   };
                    };

            this.Get["/track/lyrics/raw"] = _ =>
                {
                    return new Response
                               {
                                   ContentType = "text/plain", 
                                   Contents = stream =>
                                       {
                                           var data = Encoding.UTF8.GetBytes(this._model.Lyrics);
                                           stream.Write(data, 0, data.Length);
                                       }
                               };
                };
        }
    }
}