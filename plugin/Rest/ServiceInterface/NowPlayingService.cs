#region

using System;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
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
            string[] tracklist;

            switch (request.Type)
            {
                case MetaTag.artist:
                    tracklist = _libModule.GetArtistTracksById(request.Id);
                    break;
                case MetaTag.album:
                    tracklist = _libModule.GetAlbumTracksById(request.Id);
                    break;
                case MetaTag.genre:
                    tracklist = _libModule.GetGenreTracksById(request.Id);
                    break;
                case MetaTag.track:
                    tracklist = _libModule.GetTrackPathById(request.Id);
                    break;
                default:
                    tracklist = new string[] {};
                    break;
            }

            return new ResponseBase
            {
                Code = _module.EnqueueTracks(request.Action, tracklist) ? ApiCodes.Success : ApiCodes.Failure
            };
        }
    }
}