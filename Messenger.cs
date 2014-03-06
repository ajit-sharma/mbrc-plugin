namespace MusicBeePlugin
{
    using AndroidRemote.Entities;
    using AndroidRemote.Events;
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
            var msg = new SocketMessage(command, type, data);
            var mEvent = new MessageEvent(EventType.ReplyAvailable, msg.toJsonString());
            EventBus.FireEvent(mEvent);
        }
    }
}
