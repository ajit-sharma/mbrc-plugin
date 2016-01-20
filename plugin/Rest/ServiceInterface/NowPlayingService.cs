#region

using System;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Common;
using ServiceStack.ServiceInterface;


#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class NowPlayingService : Service
    {
        private readonly NowPlayingModule _module;
        private readonly LibraryModule _libModule;

        public NowPlayingService(NowPlayingModule module, LibraryModule libModule)
        {
            _module = module;
            _libModule = libModule;
        }

        public PaginatedResponse<NowPlaying> Get(AllNowPlaying request)
        {
            return _module.GetCurrentQueue(request.Offset, request.Limit);
        }

        public ResponseBase Put(NowPlayingPlay request)
        {
            return new ResponseBase
            {
                Code = (!string.IsNullOrEmpty(request.Path) &&
                    _module.NowplayingPlayNow(request.Path)) ? ApiCodes.Success : ApiCodes.Failure
            };
        }

        public ResponseBase Delete(NowPlayingRemove request)
        {
            return new ResponseBase
            {
                Code = _module.CurrentQueueRemoveTrack(request.Id) ? ApiCodes.Success : ApiCodes.Failure
            };
        }

        public ResponseBase Put(NowPlayingMove request)
        {
            return new ResponseBase
            {
                Code = _module.CurrentQueueMoveTrack(request.From, request.To) ? ApiCodes.Success : ApiCodes.Failure
            };
        }

        public ResponseBase Put(NowPlayingQueue request)
        {
            var tracklist = request.Path.IsNullOrEmpty() 
                ? GetTracklist(request.Type, request.Id) 
                : new []{ request.Path };
           
            return new ResponseBase
            {
                Code = _module.EnqueueTracks(request.Action, tracklist) ? ApiCodes.Success : ApiCodes.Failure
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