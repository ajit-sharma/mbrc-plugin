using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

namespace MusicBeePlugin.AndroidRemote.Commands.Internal
{
    class StartServiceBroadcast: ICommand 
    {
        private readonly ServiceDiscovery _service;

        public StartServiceBroadcast(ServiceDiscovery service)
        {
            _service = service;
        }

        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            _service.Start();
        }
    }
}
