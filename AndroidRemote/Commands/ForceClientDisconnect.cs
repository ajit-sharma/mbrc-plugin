#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class ForceClientDisconnect : ICommand
    {
        private readonly SocketServer _server;

        public ForceClientDisconnect(SocketServer server)
        {
            _server = server;
        }

        public void Execute(IEvent eEvent)
        {
            _server.KickClient(eEvent.ClientId);
        }
    }
}