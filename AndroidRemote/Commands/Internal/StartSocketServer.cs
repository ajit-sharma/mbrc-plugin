using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

namespace MusicBeePlugin.AndroidRemote.Commands.Internal
{
    class StartSocketServer:ICommand
    {
        private readonly SocketServer _server;

        public StartSocketServer (SocketServer server)
        {
            _server = server;
        }

        public void Dispose()
        {
            
        }

        public void Execute(IEvent eEvent)
        {
            _server.Start();
        }
    }
}
