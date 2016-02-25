namespace MusicBeeRemoteCore.AndroidRemote.Commands
{
    using MusicBeeRemoteCore.AndroidRemote.Interfaces;
    using MusicBeeRemoteCore.AndroidRemote.Networking;

    internal class StartSocketServer : ICommand
    {
        private readonly SocketServer _server;

        public StartSocketServer(SocketServer server)
        {
            this._server = server;
        }

        public void Execute(IEvent eEvent)
        {
            this._server.Start();
        }
    }
}