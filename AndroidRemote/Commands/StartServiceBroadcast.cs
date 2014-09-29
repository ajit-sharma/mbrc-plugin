#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class StartServiceBroadcast : ICommand
    {
        private readonly ServiceDiscovery _service;

        public StartServiceBroadcast(ServiceDiscovery service)
        {
            _service = service;
        }

        public void Execute(IEvent eEvent)
        {
            _service.Start();
        }
    }
}