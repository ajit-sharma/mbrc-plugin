#region

using MusicBeePlugin.AndroidRemote.Data;
using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.AndroidRemote.Networking;
using MusicBeePlugin.AndroidRemote.Persistence;
using MusicBeePlugin.Modules;
using Ninject.Modules;

#endregion

namespace MusicBeePlugin
{
    internal class InjectionModule : NinjectModule
    {
        public static Plugin.MusicBeeApiInterface Api;
        public static string StoragePath;

        public override void Load()
        {
            Bind<Plugin.MusicBeeApiInterface>().ToMethod(context => Api).InSingletonScope();
            Bind<CacheHelper>()
                .ToSelf()
                .WithConstructorArgument("storagePath", StoragePath);
            Bind<LibraryModule>()
                .ToSelf()
                .InSingletonScope();
            Bind<NowPlayingModule>()
                .ToSelf()
                .InSingletonScope();
            Bind<PlayerModule>()
                .ToSelf()
                .InSingletonScope();
            Bind<PlaylistModule>()
                .ToSelf()
                .InSingletonScope();
            Bind<TrackModule>()
                .ToSelf()
                .InSingletonScope();
            Bind<LyricCoverModel>()
                .ToSelf()
                .InSingletonScope();
            Bind<PersistenceController>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("storagePath", StoragePath);
            Bind<SocketServer>()
                .ToSelf()
                .InSingletonScope();
        }
    }
}