using Funq;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using System.Net;
using MusicBeePlugin.AndroidRemote.Persistence;
using ServiceStack.Api.Swagger;
using ServiceStack.Common;
using ServiceStack.Common.Web;

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

        public override void Configure(Container container)
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