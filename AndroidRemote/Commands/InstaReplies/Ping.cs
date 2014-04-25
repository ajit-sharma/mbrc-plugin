namespace MusicBeePlugin.AndroidRemote.Commands.InstaReplies
{
    using Entities;
    using Interfaces;
    using Networking;
    class Ping : ICommand
    {
        public void Dispose()
        {
        }

        public void Execute(IEvent eEvent)
        {
            var message = new SocketMessage(Constants.Ping, Constants.Reply, eEvent.Data);
            SocketServer.Instance.Send(message.toJsonString());            
        }
    }
}
