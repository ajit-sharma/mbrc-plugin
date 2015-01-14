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

        public PaginatedResponse Get(AllNowPlaying request)
        {
            return _module.GetCurrentQueue(request.offset, request.limit);
        }

        public SuccessResponse Put(NowPlayingPlay request)
        {
            return new SuccessResponse
            {
                Success =
                    !string.IsNullOrEmpty(request.path) &&
                    _module.NowplayingPlayNow(request.path)
            };
        }

        public SuccessResponse Delete(NowPlayingRemove request)
        {
            return new SuccessResponse
            {
                Success = _module.CurrentQueueRemoveTrack(request.id)
            };
        }

        public SuccessResponse Put(NowPlayingMove request)
        {
            return new SuccessResponse
            {
                Success = _module.CurrentQueueMoveTrack(request.from, request.to)
            };
        }

        public SuccessResponse Put(NowPlayingQueue request)
        {
            String[] tracklist;

            switch (request.type)
            {
                case MetaTag.artist:
                    tracklist = _libModule.GetArtistTracksById(request.id);
                    break;
                case MetaTag.album:
                    tracklist = _libModule.GetAlbumTracksById(request.id);
                    break;
                case MetaTag.genre:
                    tracklist = _libModule.GetGenreTracksById(request.id);
                    break;
                case MetaTag.track:
                    tracklist = _libModule.GetTrackPathById(request.id);
                    break;
                default:
                    tracklist = new string[] {};
                    break;
            }

            return new SuccessResponse
            {
                Success = _module.EnqueueTracks(request.action, tracklist)
            };
        }
    }
}