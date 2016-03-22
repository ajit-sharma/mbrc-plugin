namespace MusicBeeRemoteCore
{
    using MusicBeeRemoteCore.AndroidRemote.Events;
    using MusicBeeRemoteCore.AndroidRemote.Persistence;
    using MusicBeeRemoteCore.Interfaces;
    using Ninject;

    /// <summary>
    /// Entry point for the MusicBee Remote plugin.
    /// </summary>
    public interface IMusicBeeRemoteEntryPoint
    {
        /// <summary>
        /// Gets the number of the covers stored in the cache.
        /// </summary>
        int CachedCoverCount { get; }

        /// <summary>
        /// Gets the number of the tracks stored in the cache.
        /// </summary>
        int CachedTrackCount { get; }

        /// <summary>
        /// Gets the provider of the plugin's settings and configuration.
        /// </summary>
        PersistenceController Settings { get; }

        /// <summary>
        /// Gets or sets the path of the folder where the plugin will store
        /// it's settings, logs and cache information.
        /// </summary>
        string StoragePath { get; set; }

        /// <summary>
        /// Stores the base64 encoded cover image to the plugin's in memory cache.
        /// </summary>
        /// <param name="cover">The cover data encoded as a base64 string.</param>
        void CacheCover(string cover);

        /// <summary>
        /// Stores the lyrics in the plugin's in memory cache.
        /// </summary>
        /// <param name="lyrics">The lyrics of the currently playing track</param>
        void CacheLyrics(string lyrics);

        /// <summary>
        /// Gets an instance of the <see cref="EventBus"/> used for the communication internally in the plugin.
        /// </summary>
        /// <returns>The instance of the event bus.</returns>
        EventBus GetBus();

        /// <summary>
        /// Gets the <see cref="IKernel"/> used by the plugin for dependency injection.
        /// </summary>
        /// <returns>The <see cref="Ninject"/> kernel used to inject the dependencies</returns>
        IKernel GetKernel();

        /// <summary>
        /// Initialized the MusicBee plugin core.
        /// </summary>
        /// <param name="provider">
        /// The provider for the implementations of the API adapters.
        /// </param>
        void Init(IBindingProvider provider);

        /// <summary>
        /// Used to Notify the plugin for player events <see cref="AndroidRemote.Entities.NotificationMessage"/>
        /// </summary>
        /// <param name="eventType">The message type <see cref=" AndroidRemote.Entities.NotificationMessage"/></param>
        /// <param name="debounce">If true the events will be throttled every one second</param>
        void Notify(string eventType, bool debounce = false);

        /// <summary>
        /// Sets the message message handler for the plugin. The message handler is responsible 
        /// for passing messages to the host of the plugin core.
        /// </summary>
        /// <param name="messageHandler">The implementation of the <see cref="IMessageHandler"/></param>
        void SetMessageHandler(IMessageHandler messageHandler);

        /// <summary>
        /// Sets the plugin version to be displayed
        /// </summary>
        /// <param name="version">The version of the plugin</param>
        void SetVersion(string version);

        /// <summary>
        /// Handles the addition of a file in the library
        /// </summary>
        /// <param name="sourceUrl">
        /// The source url of the file.
        /// </param>
        void FileAdded(string sourceUrl);

        /// <summary>
        /// Handles the delete of a file from the library
        /// </summary>
        /// <param name="sourceUrl">
        /// The source url of the file.
        /// </param>
        void FileDeleted(string sourceUrl);

        /// <summary>
        /// Handles the change of tags on a file from the library
        /// </summary>
        /// <param name="sourceUrl">
        /// The source url of the file.
        /// </param>
        void TagsChanged(string sourceUrl);
    }
}