using Funq;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.WebHost.Endpoints;

namespace MusicBeePlugin.Rest
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost() : base("MusicBee Remote", typeof(AppHost).Assembly) {}

        public override void Configure(Container container)
        {
            Routes.Add<NowPlaying>("/nowplaying")
                .Add<NowPlaying>("/nowplaying/play");
        }
    }
}