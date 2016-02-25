namespace MusicBeeRemoteCore
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    using MusicBeeRemoteCore;
    using MusicBeeRemoteCore.AndroidRemote;
    using MusicBeeRemoteCore.AndroidRemote.Controller;
    using MusicBeeRemoteCore.AndroidRemote.Entities;
    using MusicBeeRemoteCore.AndroidRemote.Events;
    using MusicBeeRemoteCore.AndroidRemote.Model;
    using MusicBeeRemoteCore.AndroidRemote.Networking;
    using MusicBeeRemoteCore.AndroidRemote.Persistence;
    using MusicBeeRemoteCore.AndroidRemote.Utilities;
    using MusicBeeRemoteCore.Modules;

    using MusicBeeRemoteCore.Interfaces;

    using Nancy;
    using Nancy.Hosting.Self;

    using Ninject;
    using Ninject.Syntax;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public interface MusicBeeRemoteEntryPoint
    {
        string StoragePath { get; set; }

        void CacheCover(string cover);

        void CacheLyrics(string lyrics);

        EventBus getBus();

        IKernel GetKernel();

        void init(bool supportedApi, IBindingProvider provider);

        void notify(string eventType, bool debounce = false);

        void setMessageHandler(IMessageHandler messageHandler);

        void setVersion(string version);

       PersistenceController Settings { get; }

        int CachedTrackCount { get; }

        int CachedCoverCount { get; }
    }

    public class MusicBeeRemoteEntryPointImpl : MusicBeeRemoteEntryPoint
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Subject<string> eventDebouncer = new Subject<string>();

        private StandardKernel kernel;

        private IMessageHandler messageHandler;

        public PersistenceController Settings { get; private set; }

        public int CachedTrackCount
        {
            get
            {
                var module = this.kernel.Get<LibraryModule>();
                return module.GetCachedTrackCount();
            } 
        }

        public int CachedCoverCount
        {
            get
            {
                var module = this.kernel.Get<LibraryModule>();
                return module.GetCachedCoverCount();
            }
        }

        public string StoragePath { get; set; }

        public void CacheCover(string cover)
        {
            var model = this.kernel.Get<LyricCoverModel>();
            model.SetCover(cover);
        }

        public void CacheLyrics(string lyrics)
        {
            if (string.IsNullOrEmpty(lyrics))
            {
                lyrics = "Lyrics Not Found";
            }

            var model = this.kernel.Get<LyricCoverModel>();
            model.Lyrics = lyrics;
        }

        public EventBus getBus()
        {
            throw new NotImplementedException();
        }

        public IKernel GetKernel()
        {
            return this.kernel;
        }

        public void init(bool supportedApi, IBindingProvider provider)
        {
            if (!supportedApi)
            {
                return;
            }

            this.InitializeLoggingConfiguration();
            Debug.WriteLine("OOOO");

            InjectionModule.StoragePath = this.StoragePath;

            Utilities.StoragePath = this.StoragePath;

            this.kernel = new StandardKernel(new InjectionModule(provider));

            this.Settings = this.kernel.Get<PersistenceController>();
            this.Settings.LoadSettings();

            var controller = this.kernel.Get<Controller>();
            controller.InjectKernel(this.kernel);
            Configuration.Register(controller);

            var libraryModule = this.kernel.Get<LibraryModule>();
            var playlistModule = this.kernel.Get<PlaylistModule>();

            var bus = this.kernel.Get<EventBus>();

            bus.Publish(new MessageEvent(MessageEvent.ActionSocketStart));
            bus.Publish(new MessageEvent(MessageEvent.StartServiceBroadcast));
            bus.Publish(new MessageEvent(MessageEvent.ShowFirstRunDialog));

            this.BuildCache(libraryModule, playlistModule);

            this.startHttp();
            this.eventDebouncer.Throttle(TimeSpan.FromSeconds(1)).Subscribe(eventType => this.notify(eventType, false));
        }

        public void notify(string eventType, bool debounce)
        {
            if (debounce)
            {
                this.eventDebouncer.OnNext(eventType);
                return;
            }

            var server = this.kernel.Get<SocketServer>();
            var notification = new NotificationMessage { Message = eventType };

            server.Send(notification.ToJsonString());
        }

        public void setMessageHandler(IMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public void setVersion(string version)
        {
            this.Settings.Settings.CurrentVersion = version;
        }

        public void SetupModule(IBindingProvider bindingsProvider)
        {
            throw new NotImplementedException();
        }

        private void BuildCache(LibraryModule libraryModule, PlaylistModule playlistModule)
        {
            var observable = Observable.Create<string>(o =>
                {
                    if (!libraryModule.IsCacheEmpty())
                    {
                        o.OnNext("MBR: Currently building the metadata cache.");
                        libraryModule.BuildCache();
                        playlistModule.SyncPlaylistsWithCache();
                        o.OnNext("MBRC: Currently processing the album covers.");
                        libraryModule.BuildCoverCachePerAlbum();
                        o.OnNext("MBRC: Cache Ready.");
                    }
                    o.OnCompleted();
                    return () => { };
                });

            observable.Subscribe(s => this.messageHandler?.OnMessageAvailable(s), ex => Logger.Debug(ex, "Cache Build"));
        }

        /// <summary>
        ///     Initializes the logging configuration.
        /// </summary>
        private void InitializeLoggingConfiguration()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            var fileTarget = new FileTarget();
            var debugger = new DebuggerTarget();

            config.AddTarget("console", consoleTarget);
            config.AddTarget("file", fileTarget);
            config.AddTarget("debugger", debugger);

            consoleTarget.Layout = @"${date:format=HH\\:MM\\:ss} ${logger} ${message} ${exception}";
            fileTarget.FileName = $"{this.StoragePath}\\error.log";
            fileTarget.Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}||${exception}";

            debugger.Layout = fileTarget.Layout;

            var consoleRule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(consoleRule);

#if DEBUG
            var fileRule = new LoggingRule("*", LogLevel.Debug, fileTarget);
#else
			var fileRule = new LoggingRule("*", LogLevel.Error, fileTarget);
#endif
            config.LoggingRules.Add(fileRule);

            var debuggerRule = new LoggingRule("*", LogLevel.Debug, debugger);
            config.LoggingRules.Add(debuggerRule);

            LogManager.Configuration = config;
        }

        private void startHttp()
        {
            try
            {
                var bootstrapper = new Bootstrapper(this.kernel);
                //bootstrapper.Initialise();
 
                var nancyHost = new NancyHost(new Uri($"http://localhost:{this.Settings.Settings.HttpPort}/"), bootstrapper);
                nancyHost.Start();
                


            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex);
#endif
            }
        }
    }
}