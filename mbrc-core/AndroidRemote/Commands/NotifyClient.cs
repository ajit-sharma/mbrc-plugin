namespace MusicBeePlugin.AndroidRemote.Commands
{
    using MusicBeePlugin.AndroidRemote.Interfaces;
    using MusicBeePlugin.AndroidRemote.Networking;

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