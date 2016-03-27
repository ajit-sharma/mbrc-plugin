using System.Linq;

namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    using Modules;
    using ServiceModel;
    using ServiceModel.Type;
    using Nancy;
    using Nancy.ModelBinding;

    /// <summary>
    /// The playlist API module provides the playlist endpoint paths.
    /// </summary>
    public class PlaylistApiModule : NancyModule
    {
        /// <summary>
        /// The module responsible for communication with the playlist API
        /// </summary>
        private readonly PlaylistModule module;

        private readonly LibraryModule libraryModule;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        public PlaylistApiModule(PlaylistModule module, LibraryModule libraryModule)
            : base("/playlists")
        {
            this.module = module;
            this.libraryModule = libraryModule;

            this.Get["/"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    var page = this.module.GetAvailablePlaylists(limit, offset, after);
                    return this.Response.AsJson(page);
                };

            this.Get["/{id}/tracks"] = parameters =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    var page = this.module.GetPlaylistTracks((int)parameters.id, limit, offset, after);
                    return this.Response.AsJson(page);
                };

            this.Get["/trackinfo"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];
                    var page = this.module.GetPlaylistTracksInfo(limit, offset, after);
                    return this.Response.AsJson(page);
                };

            this.Put["/"] = _ =>
                {
                    var request = this.Bind<CreatePlaylist>();

                    var code = ApiCodes.MissingParameters;

                    if (request.Id > 0)
                    {
                        var tracks = libraryModule.GetTracklist(request.Type, request.Id);
                        code = this.module.CreateNewPlaylist(request.Name, tracks)
                            ? ApiCodes.Success
                            : ApiCodes.Failure;
                    }
                    else if (request.List.Length > 0)
                    {
                        code = this.module.CreateNewPlaylist(request.Name, request.List)
                            ? ApiCodes.Success
                            : ApiCodes.Failure;
                    }
                    
                    var response = new ResponseBase { Code = code };
                    return this.Response.AsJson(response);
                };

            this.Put["/play"] = _ =>
                {
                    var request = this.Bind<PlaylistPlay>();
                    var response = this.module.PlaylistPlayNow(request.Path);
                    return this.Response.AsJson(response);
                };

            this.Put["/{id}/tracks"] = parameters =>
                {
                    var request = this.Bind<AddPlaylistTracks>();
                    var code = ApiCodes.MissingParameters;

                    if (request.Id > 0)
                    {
                        var tracks = libraryModule.GetTracklist(request.Type, request.Id);
                        code = this.module.PlaylistAddTracks((int) parameters.id, request.List)
                            ? ApiCodes.Success
                            : ApiCodes.Failure;
                    }
                    else if (request.List.Length > 0)
                    {
                        code = this.module.PlaylistAddTracks((int) parameters.id, request.List)
                            ? ApiCodes.Success
                            : ApiCodes.Failure;
                    }

                    var response = new ResponseBase { Code = code };
                    return this.Response.AsJson(response);
                };

            this.Delete["/{id}"] = parameters =>
                {
                    var code = this.module.PlaylistDelete((int)parameters.id) ? ApiCodes.Success : ApiCodes.Failure;
                    var response = new ResponseBase { Code = code };
                    return this.Response.AsJson(response);
                };

            this.Delete["/{id}/tracks/{position}"] = parameters =>
                {
                    var code = this.module.DeleteTrackFromPlaylist(parameters.id, parameters.position)
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;
                    var response = new ResponseBase { Code = code };
                    return this.Response.AsJson(response);
                };

            this.Put["/{id}/tracks/{from}/{to}"] = parameters =>
                {
                    var code = this.module.MovePlaylistTrack(parameters.id, parameters.@from, parameters.to)
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;
                    var response = new ResponseBase { Code = code };
                    return this.Response.AsJson(response);
                };
        }
    }
}