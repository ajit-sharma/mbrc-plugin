using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

namespace MusicBeePlugin.AndroidRemote.Commands.Internal
{
    class RestartSocketCommand:ICommand
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

        public void Dispose()
        {
        }
    }
}
