using MusicBeePlugin.AndroidRemote.Entities;
using MusicBeePlugin.AndroidRemote.Events;

namespace MusicBeePlugin
{
    /// <summary>
    /// Class Messenger.
    /// A base class that can send messages to the socket.
    /// </summary>
    public class Messenger
    {
        /// <summary>
        /// Sends a message to the socket.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        /// <param name="client">The client.</param>
        protected void SendSocketMessage(string command, string type, object data, string client = "all")
        {
            SocketMessage msg = new SocketMessage(command, type, data);
            MessageEvent mEvent = new MessageEvent(EventType.ReplyAvailable, msg.toJsonString());
            EventBus.FireEvent(mEvent);
        }
    }
}