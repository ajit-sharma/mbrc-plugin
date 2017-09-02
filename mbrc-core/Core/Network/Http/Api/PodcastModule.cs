using Nancy;

namespace MusicBeeRemote.Core.Network.Http.Api
{
    public class PodcastModule : NancyModule
    {
        public PodcastModule() : base("/podcasts")
        {
        }
    }
}