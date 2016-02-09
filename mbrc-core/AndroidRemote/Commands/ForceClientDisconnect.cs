namespace MusicBeePlugin.AndroidRemote.Commands
{
    using MusicBeePlugin.AndroidRemote.Interfaces;
    using MusicBeePlugin.AndroidRemote.Networking;

    internal class ForceClientDisconnect : ICommand
    {
        private readonly SocketServer _server;

        public ForceClientDisconnect(SocketServer server)
        {
            this._server = server;
        }

        public void Execute(IEvent eEvent)
        {
            this._server.KickClient(eEvent.ClientId);
        }
    }
}