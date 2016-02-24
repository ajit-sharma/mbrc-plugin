namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    using MusicBeePlugin.AndroidRemote.Enumerations;
    using MusicBeePlugin.Modules;
    using MusicBeePlugin.Rest.ServiceInterface;
    using MusicBeePlugin.Rest.ServiceModel;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    /// <summary>
    /// The now playing API module provides the endpoints of related to the now playing URLs.
    /// </summary>
    public class NowPlayingApiModule : NancyModule
    {
        /// <summary>
        /// The library module provides access to the library API.
        /// </summary>
        private readonly LibraryModule libraryModule;

        /// <summary>
        /// The now playing module provides access to the now playing list.
        /// </summary>
        private readonly NowPlayingModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The now playing module.
        /// </param>
        /// <param name="libraryModule">
        /// The library module.
        /// </param>
        public NowPlayingApiModule(NowPlayingModule module, LibraryModule libraryModule)
        {
            this.module = module;
            this.libraryModule = libraryModule;

            this.Get["/nowplaying"] = o =>
                {
                    var offset = (int)this.Request.Query["offset"];
                    var limit = (int)this.Request.Query["limit"];
                    return this.Response.AsJson(this.module.GetCurrentQueue(offset, limit));
                };

            this.Put["/nowplaying/play"] = _ =>
                {
                    var data = this.Bind<NowPlayingPlay>();
                    var code = (!string.IsNullOrEmpty(data.Path) && this.module.NowplayingPlayNow(data.Path))
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };

            this.Delete["/nowplaying"] = _ =>
                {
                    var request = this.Bind<NowPlayingRemove>();
                    var code = this.module.CurrentQueueRemoveTrack(request.Id) ? ApiCodes.Success : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };

            this.Put["/nowplaying/move"] = _ =>
                {
                    var request = this.Bind<NowPlayingMove>();
                    var code = this.module.CurrentQueueMoveTrack(request.From, request.To)
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };

            this.Put["/nowplaying/queue"] = _ =>
                {
                    var request = this.Bind<NowPlayingQueue>();
                    var tracklist = string.IsNullOrEmpty(request.Path)
                                        ? this.GetTracklist(request.Type, request.Id)
                                        : new[] { request.Path };

                    var code = this.module.EnqueueTracks(request.Action, tracklist)
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };
        }

        /// <summary>
        /// Gets the track list based on the meta tag type provided and the id of the item.
        /// </summary>
        /// <param name="tag">The tag defining the type of the metadata, genre, artist etc <see cref="MetaTag"/></param>
        /// <param name="id">The id of the item in the database</param>
        /// <returns>A list of paths in the file system belonging to tracks matching the supplied parameters.</returns>
        private string[] GetTracklist(MetaTag tag, long id)
        {
            string[] tracklist;
            switch (tag)
            {
                case MetaTag.artist:
                    tracklist = this.libraryModule.GetArtistTracksById(id);
                    break;
                case MetaTag.album:
                    tracklist = this.libraryModule.GetAlbumTracksById(id);
                    break;
                case MetaTag.genre:
                    tracklist = this.libraryModule.GetGenreTracksById(id);
                    break;
                case MetaTag.track:
                    tracklist = this.libraryModule.GetTrackPathById(id);
                    break;
                default:
                    tracklist = new string[] { };
                    break;
            }

            return tracklist;
        }
    }
}