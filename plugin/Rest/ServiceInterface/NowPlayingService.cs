#region

using System;
using System.Linq;
using MusicBeePlugin.AndroidRemote.Enumerations;
using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using NServiceKit.ServiceInterface;


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

        public SuccessResponse Put(NowPlayingPlay request)
        {
            return new SuccessResponse
            {
                Success =
                    !string.IsNullOrEmpty(request.Path) &&
                    _module.NowplayingPlayNow(request.Path)
            };
        }

        public SuccessResponse Delete(NowPlayingRemove request)
        {
            return new SuccessResponse
            {
                Success = _module.CurrentQueueRemoveTrack(request.Id)
            };
        }

        public SuccessResponse Put(NowPlayingMove request)
        {
            return new SuccessResponse
            {
                Success = _module.CurrentQueueMoveTrack(request.From, request.To)
            };
        }

        public SuccessResponse Put(NowPlayingQueue request)
        {
            String[] tracklist;

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

            return new SuccessResponse
            {
                Success = _module.EnqueueTracks(request.Action, tracklist)
            };
        }
    }
}