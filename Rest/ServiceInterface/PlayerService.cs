#region

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
    internal class PlayerService : Service
    {
        private readonly PlayerModule _module;

        public PlayerService(PlayerModule module)
        {
            _module = module;
        }

        public StatusResponse Get(GetShuffleState request)
        {
            return new StatusResponse
            {
                Enabled = _module.GetShuffleState()
            };
        }

        public SuccessResponse Put(SetShuffleState request)
        {
            return new SuccessResponse
            {
                success = _module.SetShuffleState(request.enabled)
            };
        }
    }
}