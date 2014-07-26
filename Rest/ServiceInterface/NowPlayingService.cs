using System.Collections.Generic;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceInterface;

namespace MusicBeePlugin.Rest.ServiceInterface
{
    class NowPlayingService : Service
    {
        public List<NowPlaying> Get(AllNowPlaying request)
        {
            return Plugin.Instance.NowPlayingModule.GetCurrentQueue("all",request.offset,request.limit);
        }

        public object Patch(NowPlayingPlay request)
        {
            return new NowPlayingSuccessResponse()
            {
                success =
                    !string.IsNullOrEmpty(request.path) &&
                    Plugin.Instance.NowPlayingModule.NowplayingPlayNow(request.path)
            };
        }

        public object Delete(NowPlayingRemove request)
        {
            return new NowPlayingSuccessResponse()
            {
                success = Plugin.Instance.NowPlayingModule.CurrentQueueRemoveTrack(request.id)
            };
        }

        public NowPlayingSuccessResponse Patch(NowPlayingMove request)
        {
            return new NowPlayingSuccessResponse()
            {
                success = Plugin.Instance.NowPlayingModule.CurrentQueueMoveTrack(request.id, request.moveto)
            };
        }
    }
}
