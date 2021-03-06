﻿using MusicBeeRemote.Core.Feature.Library;
using MusicBeeRemote.Core.Feature.NowPlaying;
using MusicBeeRemote.Core.Network.Http.Responses;
using MusicBeeRemote.Core.Network.Http.Responses.Type;
using Nancy;
using Nancy.ModelBinding;

namespace MusicBeeRemote.Core.Network.Http.Api
{
    /// <summary>
    /// The now playing API module provides the endpoints related to the now playing URLs.
    /// </summary>
    public class NowPlayingApiModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingApiModule"/> class.
        /// </summary>
        /// <param name="module">
        /// The now playing module.
        /// </param>
        /// <param name="libraryModule">
        /// The library module.
        /// </param>
        public NowPlayingApiModule(NowPlayingModule module, LibraryModule libraryModule) : base("/nowplaying")
        {
            Get["/"] = o =>
            {
                var offset = (int) Request.Query["offset"];
                var limit = (int) Request.Query["limit"];
                return Response.AsJson(module.GetCurrentQueue(offset, limit));
            };

            Put["/play"] = _ =>
            {
                var data = this.Bind<NowPlayingPlay>();
                var code = !string.IsNullOrEmpty(data.Path) && module.NowplayingPlayNow(data.Path)
                    ? ApiCodes.Success
                    : ApiCodes.Failure;

                return Response.AsJson(new ApiResponse {Code = code});
            };

            Delete["/"] = _ =>
            {
                var request = this.Bind<NowPlayingRemove>();
                var code = module.CurrentQueueRemoveTrack(request.Id) ? ApiCodes.Success : ApiCodes.Failure;

                return Response.AsJson(new ApiResponse {Code = code});
            };

            Put["/move"] = _ =>
            {
                var request = this.Bind<NowPlayingMove>();
                var code = module.CurrentQueueMoveTrack(request.From, request.To)
                    ? ApiCodes.Success
                    : ApiCodes.Failure;

                return Response.AsJson(new ApiResponse {Code = code});
            };

            Put["/queue"] = _ =>
            {
                var request = this.Bind<NowPlayingQueue>();
                var tracklist = new[] {""};

                var code = module.EnqueueTracks(request.Action, tracklist)
                    ? ApiCodes.Success
                    : ApiCodes.Failure;

                return Response.AsJson(new ApiResponse {Code = code});
            };
        }
    }
}