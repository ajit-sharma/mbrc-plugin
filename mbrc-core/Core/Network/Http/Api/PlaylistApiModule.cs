using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Feature.Playlists;
using MusicBeeRemote.Core.Network.Http.Responses;
using MusicBeeRemote.Core.Network.Http.Responses.Type;
using Nancy;
using Nancy.ModelBinding;

namespace MusicBeeRemote.Core.Network.Http.Api
{
    /// <summary>
    /// The playlist API module provides the playlist endpoint paths.
    /// </summary>
    public class PlaylistApiModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        public PlaylistApiModule(IPlaylistModule module, LibraryModule libraryModule) : base("/playlists")
        {
            Get["/"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                var page = module.GetAvailablePlaylists(limit, offset, after);
                return Response.AsJson(page);
            };

            Get["/{id}/tracks"] = parameters =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                var page = module.GetPlaylistTracks((int) parameters.id, limit, offset, after);
                return Response.AsJson(page);
            };

            Get["/trackinfo"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];
                var page = module.GetPlaylistTracksInfo(limit, offset, after);
                return Response.AsJson(page);
            };

            Put["/"] = _ =>
            {
                var request = this.Bind<CreatePlaylist>();

                var code = ApiCodes.MissingParameters;

                if (request.Id > 0)
                {
                    var tracks = new[] {""};
                    code = module.CreateNewPlaylist(request.Name, tracks)
                        ? ApiCodes.Success
                        : ApiCodes.Failure;
                }
                else if (request.List.Length > 0)
                {
                    code = module.CreateNewPlaylist(request.Name, request.List)
                        ? ApiCodes.Success
                        : ApiCodes.Failure;
                }

                var response = new ApiResponse {Code = code};
                return Response.AsJson(response);
            };

            Put["/play"] = _ =>
            {
                var request = this.Bind<PlaylistPlay>();
                var response = new ApiResponse
                {
                    Code = module.PlaylistPlayNow(request.Path) ? ApiCodes.Success : ApiCodes.Failure
                };

                return Response.AsJson(response);
            };

            Put["/{id}/tracks"] = parameters =>
            {
                var request = this.Bind<AddPlaylistTracks>();
                var code = ApiCodes.MissingParameters;

                if (request.Id > 0)
                {
                    var tracks = new[] {""};
                    code = module.PlaylistAddTracks((int) parameters.id, tracks)
                        ? ApiCodes.Success
                        : ApiCodes.Failure;
                }
                else if (request.List.Length > 0)
                {
                    code = module.PlaylistAddTracks((int) parameters.id, request.List)
                        ? ApiCodes.Success
                        : ApiCodes.Failure;
                }

                var response = new ApiResponse {Code = code};
                return Response.AsJson(response);
            };

            Delete["/{id}"] = parameters =>
            {
                var code = module.PlaylistDelete((int) parameters.id) ? ApiCodes.Success : ApiCodes.Failure;
                var response = new ApiResponse {Code = code};
                return Response.AsJson(response);
            };

            Delete["/{id}/tracks/{position}"] = parameters =>
            {
                var code = module.DeleteTrackFromPlaylist(parameters.id, parameters.position)
                    ? ApiCodes.Success
                    : ApiCodes.Failure;
                var response = new ApiResponse {Code = code};
                return Response.AsJson(response);
            };

            Put["/{id}/tracks/{from}/{to}"] = parameters =>
            {
                var code = module.MovePlaylistTrack(parameters.id, parameters.from, parameters.to)
                    ? ApiCodes.Success
                    : ApiCodes.Failure;
                var response = new ApiResponse {Code = code};
                return Response.AsJson(response);
            };
        }
    }
}