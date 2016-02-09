namespace MusicBeePlugin.AndroidRemote.Commands
{
    using MusicBeePlugin.AndroidRemote.Interfaces;
    using MusicBeePlugin.AndroidRemote.Networking;

    internal class StopSocketServer : ICommand
    {
        private readonly SocketServer _server;

        public StopSocketServer(SocketServer server)
        {
            this._server = server;
        }

        public void Execute(IEvent eEvent)
        {
            this._server.Stop();
        }
    }
}