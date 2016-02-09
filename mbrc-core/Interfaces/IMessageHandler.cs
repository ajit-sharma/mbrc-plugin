namespace MusicBeeRemoteCore.Interfaces
{
    /// <summary>
    /// The MessageHandler interface must be implement by whoever is uses the Core library.
    /// This interface is used to pass messages to the user.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// The method will be called by the plugin library when there are informational messages available
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void OnMessageAvailable(string message);
    }
}