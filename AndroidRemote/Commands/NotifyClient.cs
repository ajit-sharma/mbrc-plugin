#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class NotifyClient : ICommand
    {
        private readonly SocketServer _server;

        public NotifyClient(SocketServer server)
        {
            _server = server;
        }

        public void Execute(IEvent eEvent)
        {
            _server.Send(eEvent.GetDataString());
        }
    }
}