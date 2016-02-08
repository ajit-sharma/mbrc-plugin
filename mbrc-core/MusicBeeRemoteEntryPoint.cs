namespace MusicBeeRemoteCore
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using MusicBeePlugin;
    using MusicBeePlugin.AndroidRemote;
    using MusicBeePlugin.AndroidRemote.Controller;
    using MusicBeePlugin.AndroidRemote.Events;
    using MusicBeePlugin.AndroidRemote.Persistence;
    using MusicBeePlugin.AndroidRemote.Utilities;
    using MusicBeePlugin.Modules;

    using Nancy.Hosting.Self;

    using Ninject;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public interface MusicBeeRemoteEntryPoint
    {
        string StoragePath { get; set; }

        void init(bool supportedApi);        
    }

    public delegate void OnStatusChanged(string message);

    public class MusicBeeRemoteEntryPointImpl : MusicBeeRemoteEntryPoint
    {
        private StandardKernel _kernel;

        private PersistenceController _persistence;

        public string StoragePath { get; set; }

        public void init(bool supportedApi)
        {

            if (!supportedApi)
            {
                return;
            }
            InitializeLoggingConfiguration();
            Debug.WriteLine("OOOO");

            InjectionModule.StoragePath = StoragePath;

            Utilities.StoragePath = StoragePath;

            _kernel = new StandardKernel(new InjectionModule());

            _persistence = _kernel.Get<PersistenceController>();
            _persistence.LoadSettings();

            var controller = _kernel.Get<Controller>();
            controller.InjectKernel(_kernel);
            Configuration.Register(controller);
            EventBus.Controller = controller;

            var libraryModule = _kernel.Get<LibraryModule>();
            var playlistModule = _kernel.Get<PlaylistModule>();

            if (libraryModule.IsCacheEmpty())
            {
                Task.Factory.StartNew(() =>
                {
                    _api.MB_SetBackgroundTaskMessage("MBR: Currently building the metadata cache.");
                    libraryModule.BuildCache();
                    playlistModule.SyncPlaylistsWithCache();
                    _api.MB_SetBackgroundTaskMessage("MBRC: Currently processing the album covers.");
                    libraryModule.BuildCoverCachePerAlbum();
                    _api.MB_SetBackgroundTaskMessage("MBRC: Cache Ready.");
                });
            }

            try
            {
                var nancyHost = new NancyHost(new Uri($"http://+:{_persistence.Settings.HttpPort}/"));
                nancyHost.Start();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex);
#endif
            }
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
            fileTarget.FileName = $"{StoragePath}\\error.log";
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
    }
}
