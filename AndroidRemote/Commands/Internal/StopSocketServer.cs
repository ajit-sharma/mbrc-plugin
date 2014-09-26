using MusicBeePlugin.AndroidRemote.Interfaces;
using MusicBeePlugin.AndroidRemote.Networking;

namespace MusicBeePlugin.AndroidRemote.Commands.Internal
{
    class StopSocketServer:ICommand
    {
        private SocketServer _server;

        public StopSocketServer(SocketServer server)
        {
            _server = server;
        }

        public void Dispose()
        {
            
        }


        public void Execute(IEvent eEvent)
        {
            _server.Stop();
        }
    }
}
