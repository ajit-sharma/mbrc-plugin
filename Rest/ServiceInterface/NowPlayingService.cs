#region

using System.Collections.Generic;
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

        public NowPlayingService(NowPlayingModule module)
        {
            _module = module;
        }

        public List<NowPlaying> Get(AllNowPlaying request)
        {
            return _module.GetCurrentQueue("all", request.offset, request.limit);
        }

        public object Patch(NowPlayingPlay request)
        {
            return new NowPlayingSuccessResponse
            {
                success =
                    !string.IsNullOrEmpty(request.path) &&
                    _module.NowplayingPlayNow(request.path)
            };
        }

        public object Delete(NowPlayingRemove request)
        {
            return new NowPlayingSuccessResponse
            {
                success = _module.CurrentQueueRemoveTrack(request.id)
            };
        }

        public NowPlayingSuccessResponse Patch(NowPlayingMove request)
        {
            return new NowPlayingSuccessResponse
            {
                success = _module.CurrentQueueMoveTrack(request.id, request.moveto)
            };
        }
    }
}