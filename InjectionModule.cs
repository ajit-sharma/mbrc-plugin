using MusicBeePlugin.AndroidRemote.Model;
using MusicBeePlugin.AndroidRemote.Networking;
using Ninject.Modules;

namespace MusicBeePlugin
{
    class InjectionModule : NinjectModule
    {
        public static Plugin.MusicBeeApiInterface Api;
        public static string StoragePath;

        public override void Load()
        {
            Bind<Plugin.MusicBeeApiInterface>().ToMethod(context => Api).InSingletonScope();
            Bind<LibraryModule>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("storagePath", StoragePath);
            Bind<NowPlayingModule>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("storagePath", StoragePath);
            Bind<PlayerModule>()
                .ToSelf()
                .InSingletonScope();
            Bind<PlaylistModule>()
                .ToSelf()
                .InSingletonScope()
                .WithConstructorArgument("storagePath", StoragePath);
            Bind<TrackModule>()
                .ToSelf()
                .InSingletonScope();
            Bind<LyricCoverModel>()
                .ToSelf()
                .InSingletonScope();
        }
    }
}
