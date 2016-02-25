namespace MusicBeeRemoteCore.AndroidRemote.Commands
{
    using MusicBeeRemoteCore.AndroidRemote.Interfaces;
    using MusicBeeRemoteCore.AndroidRemote.Networking;

    internal class NotifyClient : ICommand
    {
        private readonly SocketServer _server;

        public NotifyClient(SocketServer server)
        {
            this._server = server;
        }

        public void Execute(IEvent eEvent)
        {
            this._server.Send(eEvent.GetDataString());
        }
    }
}