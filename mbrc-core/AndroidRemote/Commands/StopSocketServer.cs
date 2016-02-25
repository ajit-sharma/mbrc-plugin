namespace MusicBeeRemoteCore.AndroidRemote.Commands
{
    using MusicBeeRemoteCore.AndroidRemote.Interfaces;
    using MusicBeeRemoteCore.AndroidRemote.Networking;

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