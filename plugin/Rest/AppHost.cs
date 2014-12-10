using Funq;
using MusicBeePlugin.AndroidRemote.Settings;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using System.Net;

namespace MusicBeePlugin.Rest
{
    public class AppHost : AppHostHttpListenerBase
    {
        private readonly SettingsController _controller;

        public AppHost(SettingsController controller)
            : base("MusicBee Remote", typeof(AppHost).Assembly)
        {
            _controller = controller;
        }

        public override void Configure(Container container)
        {
            RequestFilters.Add((req, res, requestDto) =>
            {
                var address = req.RemoteIp;
                if (!_controller.CheckIfAddressIsAllowed(address))
                {
                    res.RedirectToUrl("/", HttpStatusCode.Forbidden);
                }

            });
        }
    }
}