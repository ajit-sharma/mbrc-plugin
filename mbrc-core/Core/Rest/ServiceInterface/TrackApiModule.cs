using System.Text;
using MusicBeeRemote.Core.Model;
using MusicBeeRemote.Core.Modules;
using MusicBeeRemote.Core.Rest.ServiceModel;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using Nancy;
using Nancy.ModelBinding;

namespace MusicBeeRemote.Core.Rest.ServiceInterface
{
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

            Get["/"] = _ =>
                {
                    var trackInfo = this.module.GetTrackInfo();
                    return Response.AsJson(trackInfo);
                };

            Get["/lyrics"] = _ =>
                {
                    var response = new LyricsResponse { Lyrics = this.model.Lyrics };
                    return Response.AsJson(response);
                };

            Get["/rating"] = _ =>
                {
                    var response = new RatingResponse { Rating = this.module.GetRating() };
                    return Response.AsJson(response);
                };

            Put["/rating"] = _ =>
                {
                    var request = this.Bind<SetTrackRating>();
                    var response = new RatingResponse { Rating = this.module.SetRating(request.Rating ?? -1) };
                    return Response.AsJson(response);
                };

            Get["/position"] = _ =>
                {
                    var position = this.module.GetPosition();
                    return Response.AsJson(position);
                };

            Put["/position"] = _ =>
                {
                    var request = this.Bind<SetTrackPosition>();
                    var response = this.module.SetPosition(request.Position);
                    return Response.AsJson(response);
                };

            Get["/lfmrating"] = _ =>
                {
                    var response = new LfmRatingResponse
                                       {
                                           Status = this.module.RequestLoveStatus(string.Empty), 
                                           Code = ApiCodes.Success
                                       };
                    return Response.AsJson(response);
                };

            Put["/lfmrating"] = _ =>
                {
                    var request = this.Bind<PutLfmRating>();
                    var response = new LfmRatingResponse
                                       {
                                           Status = this.module.RequestLoveStatus(request.Status), 
                                           Code = ApiCodes.Success
                                       };
                    return Response.AsJson(response);
                };

            Get["/cover"] = _ => Response.FromStream(this.module.GetBinaryCoverData(), "image/jpeg");

            Get["/lyrics/raw"] = _ =>
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