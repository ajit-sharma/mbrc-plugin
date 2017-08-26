using MusicBeeRemoteCore.Modules;
using MusicBeeRemoteCore.Rest.ServiceModel;
using MusicBeeRemoteCore.Rest.ServiceModel.Type;
using Nancy;
using Nancy.ModelBinding;

namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
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

            Get["/"] = o =>
            {
                var offset = (int) Request.Query["offset"];
                var limit = (int) Request.Query["limit"];
                return Response.AsJson(this.module.GetCurrentQueue(offset, limit));
            };

            Put["/play"] = _ =>
            {
                var data = this.Bind<NowPlayingPlay>();
                var code = (!string.IsNullOrEmpty(data.Path) && this.module.NowplayingPlayNow(data.Path))
                    ? ApiCodes.Success
                    : ApiCodes.Failure;

                return Response.AsJson(new ResponseBase {Code = code});
            };

            Delete["/"] = _ =>
            {
                var request = this.Bind<NowPlayingRemove>();
                var code = this.module.CurrentQueueRemoveTrack(request.Id) ? ApiCodes.Success : ApiCodes.Failure;

                return Response.AsJson(new ResponseBase {Code = code});
            };

            Put["/move"] = _ =>
            {
                var request = this.Bind<NowPlayingMove>();
                var code = this.module.CurrentQueueMoveTrack(request.From, request.To)
                    ? ApiCodes.Success
                    : ApiCodes.Failure;

                return Response.AsJson(new ResponseBase {Code = code});
            };

            Put["/queue"] = _ =>
            {
                var request = this.Bind<NowPlayingQueue>();
                var tracklist = new[] {""};

                var code = this.module.EnqueueTracks(request.Action, tracklist)
                    ? ApiCodes.Success
                    : ApiCodes.Failure;

                return Response.AsJson(new ResponseBase {Code = code});
            };
        }
    }
}