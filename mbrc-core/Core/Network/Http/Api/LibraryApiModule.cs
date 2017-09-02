using System.IO;
using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Network.Http.Responses.Type;
using Nancy;

namespace MusicBeeRemote.Core.Network.Http.Api
{
    /// <summary>
    /// The library API module, provides the endpoints related with the library URLs.
    /// </summary>
    public class LibraryApiModule : NancyModule
    {
        /// <summary>
        /// The module is providing access to the player API Data
        /// </summary>
        private readonly LibraryModule _module;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        public LibraryApiModule(LibraryModule module) : base("/library")
        {
            _module = module;

            Get["/tracks"] = _ =>
                {
                    var limit = (int)Request.Query["limit"];
                    var offset = (int)Request.Query["offset"];
                    var after = (int)Request.Query["after"];

                    return Response.AsJson(_module.GetAllTracks(limit, offset, after));
                };

            Get["/tracks/{id}"] = parameters =>
                {
                    var id = (int)parameters.id;
                    return Response.AsJson(_module.GetTrackById(id));
                };

            Get["/artists"] = _ =>
                {
                    var limit = (int)Request.Query["limit"];
                    var offset = (int)Request.Query["offset"];
                    var after = (int)Request.Query["after"];

                    return Response.AsJson(_module.GetAllArtists(limit, offset, after));
                };

            Get["/artists/{id}"] = parameters =>
                {
                    var id = (int)parameters.id;
                    return Response.AsJson(_module.GetArtistById(id));
                };

            Get["/genres"] = _ =>
                {
                    var limit = (int)Request.Query["limit"];
                    var offset = (int)Request.Query["offset"];
                    var after = (int)Request.Query["after"];

                    return Response.AsJson(_module.GetAllGenres(limit, offset, after));
                };

            Get["/albums"] = _ =>
                {
                    var limit = (int)Request.Query["limit"];
                    var offset = (int)Request.Query["offset"];
                    var after = (int)Request.Query["after"];

                    return Response.AsJson(_module.GetAllAlbums(limit, offset, after));
                };

            Get["/covers"] = _ =>
                {
                    var limit = (int)Request.Query["limit"];
                    var offset = (int)Request.Query["offset"];
                    var after = (int)Request.Query["after"];
                    return Response.AsJson(_module.GetAllCovers(offset, limit, after));
                };

            Get["/covers/{id}"] = parameters =>
                {
                    var id = (int)parameters.id;
                    return Response.AsJson(_module.GetLibraryCover(id, true));
                };

            Get["/covers/{id}/raw"] = parameters =>
            {

                Response response;
                try
                {
                    response = Response.FromStream(_module.GetCoverData((int)parameters.id), "image/jpeg");
                }
                catch (FileNotFoundException)
                {
                    response = Response.AsJson(new ResponseBase
                    {
                        Code = ApiCodes.NotFound
                    });
                    response.StatusCode = HttpStatusCode.NotFound;
                }
                
                return response;
            };
        }
    }
}