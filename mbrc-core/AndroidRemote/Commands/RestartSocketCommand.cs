namespace MusicBeePlugin.AndroidRemote.Commands
{
    using MusicBeePlugin.AndroidRemote.Interfaces;
    using MusicBeePlugin.AndroidRemote.Networking;

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