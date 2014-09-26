namespace MusicBeePlugin.AndroidRemote.Commands.Internal
{
    using Interfaces;
    using Networking;

    class ForceClientDisconnect:ICommand
    {
        private readonly SocketServer _server;

        public ForceClientDisconnect(SocketServer server)
        {
            _server = server;
        }

        public void Dispose()
        {
            
        }

        public void Execute(IEvent eEvent)
        {
            _server.KickClient(eEvent.ClientId);
        }
    }
}
