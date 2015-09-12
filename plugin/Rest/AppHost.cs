
using System.Net;
using MusicBeePlugin.AndroidRemote.Persistence;
using NServiceKit.Api.Swagger;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;


namespace MusicBeePlugin.Rest
{
    public class AppHost : AppHostHttpListenerBase
    {
        private readonly PersistenceController _controller;

        public AppHost(PersistenceController controller)
            : base("MusicBee Remote", typeof(AppHost).Assembly)
        {
            _controller = controller;
        }
        
        public override void Configure(Funq.Container container)
        {
            SetConfig(new EndpointHostConfig()
            {
                EnableFeatures = Feature.All.Remove(Feature.Csv |
                                               Feature.Jsv |
                                               Feature.Soap |
                                               Feature.Soap11 |
                                               Feature.Soap12 |
                                               Feature.Xml),

                DefaultContentType = MimeTypes.Json,
#if DEBUG
                // Show StackTraces in service responses during development
                DebugMode = true,
#endif
            });

            Plugins.Add(new SwaggerFeature()
            {
                UseLowercaseUnderscoreModelPropertyNames = true
            });
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