using System.Text;
using MusicBeeRemote.Core.Feature;
using MusicBeeRemote.Core.Feature.NowPlaying;
using MusicBeeRemote.Core.Network.Http.Responses;
using MusicBeeRemote.Core.Network.Http.Responses.Type;
using Nancy;
using Nancy.ModelBinding;

namespace MusicBeeRemote.Core.Network.Http.Api
{
    /// <summary>
    /// The track API module provides the track endpoint functionality.
    /// </summary>
    public class TrackApiModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        public TrackApiModule(TrackModule module, LyricCoverModel model) : base("/track")
        {
            Get["/"] = _ =>
                {
                    var trackInfo = module.GetTrackInfo();
                    return Response.AsJson(trackInfo);
                };

            Get["/lyrics"] = _ =>
                {
                    var response = new LyricsResponse { Lyrics = model.Lyrics };
                    return Response.AsJson(response);
                };

            Get["/rating"] = _ =>
                {
                    var response = new RatingResponse { Rating = module.GetRating() };
                    return Response.AsJson(response);
                };

            Put["/rating"] = _ =>
                {
                    var request = this.Bind<SetTrackRating>();
                    var response = new RatingResponse { Rating = module.SetRating(request.Rating ?? -1) };
                    return Response.AsJson(response);
                };

            Get["/position"] = _ =>
                {
                    var position = module.GetPosition();
                    return Response.AsJson(position);
                };

            Put["/position"] = _ =>
                {
                    var request = this.Bind<SetTrackPosition>();
                    var response = module.SetPosition(request.Position);
                    return Response.AsJson(response);
                };

            Get["/lfmrating"] = _ =>
                {
                    var response = new LfmRatingResponse
                                       {
                                           Status = module.RequestLoveStatus(string.Empty), 
                                           Code = ApiCodes.Success
                                       };
                    return Response.AsJson(response);
                };

            Put["/lfmrating"] = _ =>
                {
                    var request = this.Bind<PutLfmRating>();
                    var response = new LfmRatingResponse
                                       {
                                           Status = module.RequestLoveStatus(request.Status), 
                                           Code = ApiCodes.Success
                                       };
                    return Response.AsJson(response);
                };

            Get["/cover"] = _ => Response.FromStream(module.GetBinaryCoverData(), "image/jpeg");

            Get["/lyrics/raw"] = _ => new Response
            {
                ContentType = "text/plain", 
                Contents = stream =>
                {
                    var data1 = Encoding.UTF8.GetBytes(model.Lyrics);
                    stream.Write(data1, 0, data1.Length);
                }
            };
        }
    }
}