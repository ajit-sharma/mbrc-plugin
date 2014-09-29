#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class StartSocketServer : ICommand
    {
        private readonly SocketServer _server;

        public StartSocketServer(SocketServer server)
        {
            _server = server;
        }

        public void Execute(IEvent eEvent)
        {
            _server.Start();
        }
    }
}