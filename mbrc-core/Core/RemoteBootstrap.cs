using System.Reactive.Concurrency;
using MusicBeeRemote.Core.ApiAdapters;
using MusicBeeRemote.Core.Logging;
using MusicBeeRemote.Core.Model;
using MusicBeeRemote.Core.Network;
using MusicBeeRemote.Core.Settings;
using MusicBeeRemote.Core.Settings.Dialog.BasePanel;
using MusicBeeRemote.Core.Settings.Dialog.Commands;
using MusicBeeRemote.Core.Settings.Dialog.Whitelist;
using MusicBeeRemote.Core.Windows;
using MusicBeeRemoteData;
using MusicBeeRemoteData.Repository;
using MusicBeeRemoteData.Repository.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StructureMap;
using TinyMessenger;

namespace MusicBeeRemote.Core
{
    public class RemoteBootstrap
    {
        private readonly Container _container;

        public RemoteBootstrap()
        {
            _container = new Container();
        }

        public IMusicBeeRemotePlugin BootStrap(MusicBeeDependencies dependencies)
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    NullValueHandling = NullValueHandling.Ignore
                };

                settings.Converters.Add(new StringEnumConverter {CamelCaseText = false});
                return settings;
            };

            _container.Configure(c =>
            {
                c.For<ILibraryApiAdapter>().Use(() => dependencies.LibraryAdapter).Singleton();
                c.For<INowPlayingApiAdapter>().Use(() => dependencies.NowPlayingAdapter).Singleton();
                c.For<IOutputApiAdapter>().Use(() => dependencies.OutputAdapter).Singleton();
                c.For<IPlayerApiAdapter>().Use(() => dependencies.PlayerAdapter).Singleton();
                c.For<IQueueAdapter>().Use(() => dependencies.QueueAdapter).Singleton();
                c.For<ITrackApiAdapter>().Use(() => dependencies.TrackAdapter).Singleton();
                c.For<IInvokeHandler>().Use(() => dependencies.InvokeHandler).Singleton();

                c.For<IWindowManager>().Use<WindowManager>().Singleton();

                c.For<IPluginLogManager>().Use<PluginLogManager>().Singleton();

                c.For<SocketServer>().Use<SocketServer>().Singleton();
                c.For<LyricCoverModel>().Use<LyricCoverModel>().Singleton();
                c.For<ServiceDiscovery>().Use<ServiceDiscovery>().Singleton();

                c.For<PersistenceManager>().Use<PersistenceManager>().Singleton();
                c.For<IJsonSettingsFileManager>().Use<JsonSettingsFileManager>().Singleton();               

                c.For<IStorageLocationProvider>()
                    .Use<StorageLocationProvider>()
                    .Ctor<string>()
                    .Is(dependencies.BaseStoragePath)
                    .Singleton();                 

                c.For<IVersionProvider>()
                    .Use<VersionProvider>()
                    .Ctor<string>()
                    .Is(dependencies.CurrentVersion)
                    .Singleton();

                c.For<IScheduler>().Use(() => ThreadPoolScheduler.Instance)
                    .Singleton();
                c.For<ITrackRepository>().Use<TrackRepository>().Singleton();
                c.For<IGenreRepository>().Use<GenreRepository>().Singleton();
                c.For<ITinyMessengerHub>().Use<TinyMessengerHub>().Singleton();
                c.For<IMusicBeeRemotePlugin>().Use<MusicBeeRemotePlugin>().Singleton();

                c.For<OpenHelpCommand>().Use<OpenHelpCommand>();
                c.For<OpenLogDirectoryCommand>().Use<OpenLogDirectoryCommand>();
                c.For<SaveConfigurationCommand>().Use<SaveConfigurationCommand>();
                c.For<ConfigurationPanel>().Use<ConfigurationPanel>();
                c.For<ConfigurationPanelViewModel>().Use<ConfigurationPanelViewModel>();
                c.For<IConfigurationPanelPresenter>().Use<ConfigurationPanelPresenter>();

                c.For<IWhitelistManagementPresenter>().Use<WhitelistManagementPresenter>();
                c.For<WhitelistManagementControl>().Use<WhitelistManagementControl>();
                
                c.For<Bootstrapper>().Use(() => new Bootstrapper(_container)).Singleton();
            });

            return _container.GetInstance<IMusicBeeRemotePlugin>();
        }
    }
}