namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    using System.Text;

    using MusicBeeRemoteCore.AndroidRemote.Model;
    using MusicBeeRemoteCore.Modules;
    using MusicBeeRemoteCore.Rest.ServiceModel;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    /// <summary>
    /// The track API module provides the track endpoint functionality.
    /// </summary>
    public class TrackApiModule : NancyModule
    {
        /// <summary>
        /// The model that store information about the playing tracks lyrics and cover.
        /// </summary>
        private readonly LyricCoverModel model;

        /// <summary>
        /// The module that provides the track API functionality.
        /// </summary>
        private readonly TrackModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        public TrackApiModule(TrackModule module, LyricCoverModel model)
            : base("/track")
        {
            this.module = module;
            this.model = model;

            this.Get["/"] = _ =>
                {
                    var trackInfo = this.module.GetTrackInfo();
                    return this.Response.AsJson(trackInfo);
                };

            this.Get["/lyrics"] = _ =>
                {
                    var response = new LyricsResponse { Lyrics = this.model.Lyrics };
                    return this.Response.AsJson(response);
                };

            this.Get["/rating"] = _ =>
                {
                    var response = new RatingResponse { Rating = this.module.GetRating() };
                    return this.Response.AsJson(response);
                };

            this.Put["/rating"] = _ =>
                {
                    var request = this.Bind<SetTrackRating>();
                    var response = new RatingResponse { Rating = this.module.SetRating(request.Rating ?? -1) };
                    return this.Response.AsJson(response);
                };

            this.Get["/position"] = _ =>
                {
                    var position = this.module.GetPosition();
                    return this.Response.AsJson(position);
                };

            this.Put["/position"] = _ =>
                {
                    var request = this.Bind<SetTrackPosition>();
                    var response = this.module.SetPosition(request.Position);
                    return this.Response.AsJson(response);
                };

            this.Get["/lfmrating"] = _ =>
                {
                    var response = new LfmRatingResponse
                                       {
                                           Status = this.module.RequestLoveStatus(string.Empty), 
                                           Code = ApiCodes.Success
                                       };
                    return this.Response.AsJson(response);
                };

            this.Put["/lfmrating"] = _ =>
                {
                    var request = this.Bind<PutLfmRating>();
                    var response = new LfmRatingResponse
                                       {
                                           Status = this.module.RequestLoveStatus(request.Status), 
                                           Code = ApiCodes.Success
                                       };
                    return this.Response.AsJson(response);
                };

            this.Get["/cover"] = _ => this.Response.FromStream(this.module.GetBinaryCoverData(), "image/jpeg");

            this.Get["/lyrics/raw"] = _ =>
                {
                    var response = new Response
                                       {
                                           ContentType = "text/plain", 
                                           Contents = stream =>
                                               {
                                                   var data = Encoding.UTF8.GetBytes(this.model.Lyrics);
                                                   stream.Write(data, 0, data.Length);
                                               }
                                       };
                    return response;
                };
        }
    }
}