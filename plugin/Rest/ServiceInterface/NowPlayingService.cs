#region

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
    }
}