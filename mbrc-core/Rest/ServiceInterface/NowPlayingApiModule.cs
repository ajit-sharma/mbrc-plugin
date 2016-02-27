namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    using MusicBeeRemoteCore.Modules;
    using MusicBeeRemoteCore.Rest.ServiceModel;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using Nancy;
    using Nancy.ModelBinding;

    /// <summary>
    /// The now playing API module provides the endpoints related to the now playing URLs.
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
            : base("/nowplaying")
        {
            this.module = module;
            this.libraryModule = libraryModule;

            this.Get["/"] = o =>
                {
                    var offset = (int)this.Request.Query["offset"];
                    var limit = (int)this.Request.Query["limit"];
                    return this.Response.AsJson(this.module.GetCurrentQueue(offset, limit));
                };

            this.Put["/play"] = _ =>
                {
                    var data = this.Bind<NowPlayingPlay>();
                    var code = (!string.IsNullOrEmpty(data.Path) && this.module.NowplayingPlayNow(data.Path))
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };

            this.Delete["/"] = _ =>
                {
                    var request = this.Bind<NowPlayingRemove>();
                    var code = this.module.CurrentQueueRemoveTrack(request.Id) ? ApiCodes.Success : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };

            this.Put["/move"] = _ =>
                {
                    var request = this.Bind<NowPlayingMove>();
                    var code = this.module.CurrentQueueMoveTrack(request.From, request.To)
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };

            this.Put["/queue"] = _ =>
                {
                    var request = this.Bind<NowPlayingQueue>();
                    var tracklist = string.IsNullOrEmpty(request.Path)
                                        ? this.libraryModule.GetTracklist(request.Type, request.Id)
                                        : new[] { request.Path };

                    var code = this.module.EnqueueTracks(request.Action, tracklist)
                                   ? ApiCodes.Success
                                   : ApiCodes.Failure;

                    return this.Response.AsJson(new ResponseBase { Code = code });
                };
        }
    }
}