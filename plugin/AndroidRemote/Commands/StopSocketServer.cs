#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class StopSocketServer : ICommand
    {
        private readonly SocketServer _server;

        public StopSocketServer(SocketServer server)
        {
            _server = server;
        }


        public void Execute(IEvent eEvent)
        {
            _server.Stop();
        }
    }
}