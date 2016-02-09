namespace MusicBeePlugin.Rest.ServiceInterface
{
    using MusicBeePlugin.Modules;
    using MusicBeePlugin.Rest.ServiceModel;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    internal class PlaylistApiModule : NancyModule
    {
        private readonly PlaylistModule _module;

        public PlaylistApiModule(PlaylistModule module)
        {
            this._module = module;

            this.Get["/playlists"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    return this._module.GetAvailablePlaylists(limit, offset, after);
                };

            this.Get["/playlists/{id}/tracks"] = parameters =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];

                    return this._module.GetPlaylistTracks(parameters.id, limit, offset, after);
                };

            this.Get["/playlists/trackinfo"] = _ =>
                {
                    var limit = (int)this.Request.Query["limit"];
                    var offset = (int)this.Request.Query["offset"];
                    var after = (int)this.Request.Query["after"];
                    return this._module.GetPlaylistTracksInfo(limit, offset, after);
                };

            this.Put["/playlists"] = _ =>
                {
                    var request = this.Bind<CreatePlaylist>();
                    return this._module.CreateNewPlaylist(request.Name, request.List);
                };

            this.Put["/playlists/play"] = _ =>
                {
                    var request = this.Bind<PlaylistPlay>();
                    return this._module.PlaylistPlayNow(request.Path);
                };

            this.Put["/playlists/{id}/tracks"] = parameters =>
                {
                    var request = this.Bind<AddPlaylistTracks>();

                    return new ResponseBase
                               {
                                   Code =
                                       this._module.PlaylistAddTracks(parameters.id, request.List)
                                           ? ApiCodes.Success
                                           : ApiCodes.Failure
                               };
                };

            this.Delete["/playlists/{id}"] =
                parameters =>
                new ResponseBase
                    {
                        Code =
                            this._module.PlaylistDelete(parameters.id)
                                ? ApiCodes.Success
                                : ApiCodes.Failure
                    };

            this.Delete["/playlists/{id}/tracks/{position}"] =
                parameters =>
                new ResponseBase
                    {
                        Code =
                            this._module.DeleteTrackFromPlaylist(parameters.id, parameters.position)
                                ? ApiCodes.Success
                                : ApiCodes.Failure
                    };

            this.Put["/playlists/{id}/tracks/{from}/{to}"] =
                parameters =>
                new ResponseBase
                    {
                        Code =
                            this._module.MovePlaylistTrack(parameters.id, parameters.@from, parameters.to)
                                ? ApiCodes.Success
                                : ApiCodes.Failure
                    };
        }
    }
}