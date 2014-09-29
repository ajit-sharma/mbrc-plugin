#region

using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

#endregion

namespace MusicBeePlugin.AndroidRemote.Commands
{
    internal class RestartSocketCommand : ICommand
    {
        private readonly SocketServer _server;

        public RestartSocketCommand(SocketServer server)
        {
            _server = server;
        }

        public void Execute(IEvent eEvent)
        {
            _server.RestartSocket();
        }
    }
}