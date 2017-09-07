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
        /// Initializes a new instance of the <see cref="LibraryApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        public LibraryApiModule(LibraryModule module) : base("/library")
        {
            Get["/tracks"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return Response.AsJson(module.GetAllTracks(limit, offset, after));
            };

            Get["/tracks/{id}"] = parameters =>
            {
                var id = (int) parameters.id;
                return Response.AsJson(module.GetTrackById(id));
            };

            Get["/artists"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return Response.AsJson(module.GetAllArtists(limit, offset, after));
            };

            Get["/artists/{id}"] = parameters =>
            {
                var id = (int) parameters.id;
                return Response.AsJson(module.GetArtistById(id));
            };

            Get["/genres"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return Response.AsJson(module.GetAllGenres(limit, offset, after));
            };

            Get["/albums"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return Response.AsJson(module.GetAllAlbums(limit, offset, after));
            };

            Get["/covers"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];
                return Response.AsJson(module.GetAllCovers(offset, limit, after));
            };

            Get["/covers/{id}"] = parameters =>
            {
                var id = (int) parameters.id;
                return Response.AsJson(module.GetLibraryCover(id, true));
            };

            Get["/covers/{id}/raw"] = parameters =>
            {
                Response response;
                try
                {
                    response = Response.FromStream(module.GetCoverData((int) parameters.id), "image/jpeg");
                }
                catch (FileNotFoundException)
                {
                    response = Response.AsJson(new ApiResponse
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