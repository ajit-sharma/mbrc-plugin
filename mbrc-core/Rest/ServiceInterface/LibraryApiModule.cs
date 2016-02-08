#region Dependencies

using MusicBeePlugin.Modules;
using Nancy;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class LibraryApiModule : NancyModule
    {
        private readonly LibraryModule _module;

        public LibraryApiModule(LibraryModule module)
        {
            _module = module;

            Get["/library/tracks"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return _module.GetAllTracks(limit, offset, after);
            };

            Get["/library/tracks/{id}"] = parameters => _module.GetTrackById(parameters.id);

            Get["/library/artists"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return _module.GetAllArtists(limit, offset, after);
            };

            Get["/library/artists/{id}"] = parameters => _module.GetArtistById(parameters.id);

            Get["/library/genres"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return _module.GetAllGenres(limit, offset, after);
            };

            Get["/library/albums"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return _module.GetAllAlbums(limit, offset, after);
            };

            Get["/library/covers"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];
                return _module.GetAllCovers(limit, offset, after);
            };

            Get["/library/covers/{id}"] = parameters => _module.GetLibraryCover(parameters.id, true);

            Get["/library/covers/{id}/raw"] = parameters =>
            {
                return new Response
                {
                    ContentType = "image/jpeg",
                    Contents = stream => _module.GetCoverData(parameters.id)
                };
            };
        }
    }
}