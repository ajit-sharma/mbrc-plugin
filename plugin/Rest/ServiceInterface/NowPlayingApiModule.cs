#region

using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Nancy;
using Nancy.ModelBinding;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class NowPlayingApiModule : NancyModule
    {
        private readonly NowPlayingModule _module;
        private readonly LibraryModule _libModule;

        public NowPlayingApiModule(NowPlayingModule module, LibraryModule libModule)
        {
            _module = module;
            _libModule = libModule;

            Get["/nowplaying"] = o =>
            {
                var offset = (int) Request.Query["offset"];
                var limit = (int) Request.Query["limit"];
                return _module.GetCurrentQueue(offset, limit);
            };

            Put["/nowplaying/play"] = _ =>
            {
                var data = this.Bind<NowPlayingPlay>();
                return new ResponseBase
                {
                    Code = (!string.IsNullOrEmpty(data.Path) &&
                            _module.NowplayingPlayNow(data.Path))
                        ? ApiCodes.Success
                        : ApiCodes.Failure
                };
            };

            Delete["/nowplaying"] = _ =>
            {
                var request = this.Bind<NowPlayingRemove>();
                return new ResponseBase
                {
                    Code = _module.CurrentQueueRemoveTrack(request.Id) ? ApiCodes.Success : ApiCodes.Failure
                };
            };

            Put["/nowplaying/move"] = _ =>
            {
                var request = this.Bind<NowPlayingMove>();
                return new ResponseBase
                {
                    Code = _module.CurrentQueueMoveTrack(request.From, request.To) ? ApiCodes.Success : ApiCodes.Failure
                };
            };

            Put["/nowplaying/queue"] = _ =>
            {
                var request = this.Bind<NowPlayingQueue>();
                var tracklist = string.IsNullOrEmpty(request.Path)
                    ? GetTracklist(request.Type, request.Id)
                    : new[] {request.Path};

                return new ResponseBase
                {
                    Code = _module.EnqueueTracks(request.Action, tracklist) ? ApiCodes.Success : ApiCodes.Failure
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
                    tracklist = _libModule.GetArtistTracksById(id);
                    break;
                case MetaTag.album:
                    tracklist = _libModule.GetAlbumTracksById(id);
                    break;
                case MetaTag.genre:
                    tracklist = _libModule.GetGenreTracksById(id);
                    break;
                case MetaTag.track:
                    tracklist = _libModule.GetTrackPathById(id);
                    break;
                default:
                    tracklist = new string[] {};
                    break;
            }
            return tracklist;
        }
    }
}