
namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    using MusicBeePlugin.Modules;

    using Nancy;

    /// <summary>
    /// The library API module, provides the endpoints related with the library URLs.
    /// </summary>
    public class LibraryApiModule : NancyModule
    {
        /// <summary>
        /// The module is providing access to the player API Data
        /// </summary>
        private readonly LibraryModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        public LibraryApiModule(LibraryModule module)
        {
            this.module = module;

            this.Get["/library/tracks"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    return this.Response.AsJson(this.module.GetAllTracks(limit, offset, after));
                };

            this.Get["/library/tracks/{id}"] = parameters =>
                {
                    var id = (int)parameters.id;
                    return this.Response.AsJson(this.module.GetTrackById(id));
                };

            this.Get["/library/artists"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    return this.Response.AsJson(this.module.GetAllArtists(limit, offset, after));
                };

            this.Get["/library/artists/{id}"] = parameters =>
                {
                    var id = (int)parameters.id;
                    return this.Response.AsJson(this.module.GetArtistById(id));
                };

            this.Get["/library/genres"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    return this.Response.AsJson(this.module.GetAllGenres(limit, offset, after));
                };

            this.Get["/library/albums"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    return this.Response.AsJson(this.module.GetAllAlbums(limit, offset, after));
                };

            this.Get["/library/covers"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];
                    return this.Response.AsJson(this.module.GetAllCovers(limit, offset, after));
                };

            this.Get["/library/covers/{id}"] = parameters =>
                {
                    var id = (int)parameters.id;
                    return this.Response.AsJson(this.module.GetLibraryCover(id, true));
                };

            this.Get["/library/covers/{id}/raw"] =
                parameters =>
                    {
                        return new Response
                                   {
                                       ContentType = "image/jpeg", 
                                       Contents = stream => this.module.GetCoverData(parameters.id)
                                   };
                    };
        }
    }
}