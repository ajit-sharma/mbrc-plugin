namespace MusicBeePlugin.AndroidRemote.Commands
{
    using MusicBeePlugin.AndroidRemote.Interfaces;
    using MusicBeePlugin.AndroidRemote.Networking;

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