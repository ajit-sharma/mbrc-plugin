namespace MusicBeeRemoteCore.AndroidRemote.Commands
{
    using MusicBeeRemoteCore.AndroidRemote.Interfaces;
    using MusicBeeRemoteCore.AndroidRemote.Networking;

    internal class RestartSocketCommand : ICommand
    {
        private readonly SocketServer _server;

        public RestartSocketCommand(SocketServer server)
        {
            this._server = server;
        }

        public void Execute(IEvent eEvent)
        {
            this._server.RestartSocket();
        }
    }
}