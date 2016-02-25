namespace MusicBeeRemoteCore.AndroidRemote.Commands
{
    using MusicBeeRemoteCore.AndroidRemote.Interfaces;
    using MusicBeeRemoteCore.AndroidRemote.Networking;

    internal class StartServiceBroadcast : ICommand
    {
        private readonly ServiceDiscovery _service;

        public StartServiceBroadcast(ServiceDiscovery service)
        {
            this._service = service;
        }

        public void Execute(IEvent eEvent)
        {
            this._service.Start();
        }
    }
}