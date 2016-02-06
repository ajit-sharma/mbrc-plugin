#region

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Nancy;
using Nancy.ModelBinding;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class PlaylistApiModule : NancyModule
    {
        private readonly PlaylistModule _module;

        public PlaylistApiModule(PlaylistModule module)
        {
            _module = module;

            Get["/playlists"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return _module.GetAvailablePlaylists(limit, offset, after);
            };

            Get["/playlists/{id}/tracks"] = parameters =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];

                return _module.GetPlaylistTracks(parameters.id, limit, offset, after);
            };

            Get["/playlists/trackinfo"] = _ =>
            {
                var limit = (int) Request.Query["limit"];
                var offset = (int) Request.Query["offset"];
                var after = (int) Request.Query["after"];
                return _module.GetPlaylistTracksInfo(limit, offset, after);
            };

            Put["/playlists"] = _ =>
            {
                var request = this.Bind<CreatePlaylist>();
                return _module.CreateNewPlaylist(request.Name, request.List);
            };

            Put["/playlists/play"] = _ =>
            {
                var request = this.Bind<PlaylistPlay>();
                return _module.PlaylistPlayNow(request.Path);
            };

            Put["/playlists/{id}/tracks"] = parameters =>
            {
                var request = this.Bind<AddPlaylistTracks>();

                return new ResponseBase
                {
                    Code = _module.PlaylistAddTracks(parameters.id, request.List) ? ApiCodes.Success : ApiCodes.Failure
                };
            };

            Delete["/playlists/{id}"] = parameters => new ResponseBase
            {
                Code = _module.PlaylistDelete(parameters.id) ? ApiCodes.Success : ApiCodes.Failure
            };

            Delete["/playlists/{id}/tracks/{position}"] = parameters => new ResponseBase
            {
                Code = _module.DeleteTrackFromPlaylist(parameters.id, parameters.position) ? ApiCodes.Success : ApiCodes.Failure
            };

            Put["/playlists/{id}/tracks/{from}/{to}"] = parameters => new ResponseBase
            {
                Code =
                    _module.MovePlaylistTrack(parameters.id, parameters.@from, parameters.to)
                        ? ApiCodes.Success
                        : ApiCodes.Failure
            };

        }
	}
}