#region

using System.Collections.Generic;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Ninject;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class NowPlayingService : Service
    {
        private readonly NowPlayingModule _module;

        public NowPlayingService()
        {
            using (var kernel = new StandardKernel(new InjectionModule()))
            {
                _module = kernel.Get<NowPlayingModule>();
            }
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