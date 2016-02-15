namespace MusicBeePlugin.Rest.ServiceInterface
{
    using MusicBeePlugin.AndroidRemote.Enumerations;
    using MusicBeePlugin.Modules;
    using MusicBeePlugin.Rest.ServiceModel;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    public class NowPlayingApiModule : NancyModule
    {
        private readonly LibraryModule _libModule;

        private readonly NowPlayingModule _module;

        public NowPlayingApiModule(NowPlayingModule module, LibraryModule libModule)
        {
            this._module = module;
            this._libModule = libModule;

            this.Get["/nowplaying"] = o =>
                {
                    var offset = (int)this.Request.Query["offset"];
                    var limit = (int)this.Request.Query["limit"];
                    return this._module.GetCurrentQueue(offset, limit);
                };

            this.Put["/nowplaying/play"] = _ =>
                {
                    var data = this.Bind<NowPlayingPlay>();
                    return new ResponseBase
                               {
                                   Code =
                                       (!string.IsNullOrEmpty(data.Path)
                                        && this._module.NowplayingPlayNow(data.Path))
                                           ? ApiCodes.Success
                                           : ApiCodes.Failure
                               };
                };

            this.Delete["/nowplaying"] = _ =>
                {
                    var request = this.Bind<NowPlayingRemove>();
                    return new ResponseBase
                               {
                                   Code =
                                       this._module.CurrentQueueRemoveTrack(request.Id)
                                           ? ApiCodes.Success
                                           : ApiCodes.Failure
                               };
                };

            this.Put["/nowplaying/move"] = _ =>
                {
                    var request = this.Bind<NowPlayingMove>();
                    return new ResponseBase
                               {
                                   Code =
                                       this._module.CurrentQueueMoveTrack(request.From, request.To)
                                           ? ApiCodes.Success
                                           : ApiCodes.Failure
                               };
                };

            this.Put["/nowplaying/queue"] = _ =>
                {
                    var request = this.Bind<NowPlayingQueue>();
                    var tracklist = string.IsNullOrEmpty(request.Path)
                                        ? this.GetTracklist(request.Type, request.Id)
                                        : new[] { request.Path };

                    return new ResponseBase
                               {
                                   Code =
                                       this._module.EnqueueTracks(request.Action, tracklist)
                                           ? ApiCodes.Success
                                           : ApiCodes.Failure
                               };
                };
        }

        /// <summary>
        /// Gets the tracklist based on the meta tag type provided and the id of the item.
        /// </summary>
        /// <param name="tag">The tag defining the type of the metadata, genre, artist etc <see cref="MetaTag"/></param>
        /// <param name="id">The id of the item in the database</param>
        /// <returns></returns>
        private string[] GetTracklist(MetaTag tag, long id)
        {
            string[] tracklist;
            switch (tag)
            {
                case MetaTag.artist:
                    tracklist = this._libModule.GetArtistTracksById(id);
                    break;
                case MetaTag.album:
                    tracklist = this._libModule.GetAlbumTracksById(id);
                    break;
                case MetaTag.genre:
                    tracklist = this._libModule.GetGenreTracksById(id);
                    break;
                case MetaTag.track:
                    tracklist = this._libModule.GetTrackPathById(id);
                    break;
                default:
                    tracklist = new string[] { };
                    break;
            }

            return tracklist;
        }
    }
}