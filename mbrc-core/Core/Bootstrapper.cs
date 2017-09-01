using MusicBeeRemote.Core.Rest.Compression;
using MusicBeeRemote.Core.Rest.ServiceInterface;
using MusicBeeRemote.Core.Rest.ServiceModel.Type;
using MusicBeeRemote.Core.Rest.StatusCodeHandlers;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.Responses;
using StructureMap;

namespace MusicBeeRemote.Core
{
    public class Bootstrapper : StructureMapNancyBootstrapper
    {
        private readonly Container _container;

        public Bootstrapper(Container existingContainer)
        {
            _container = existingContainer;
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration => new DiagnosticsConfiguration
        {
            Password = "12345"
        };

        protected override NancyInternalConfiguration InternalConfiguration => NancyInternalConfiguration
            .WithOverrides(configuration =>
            {
                configuration.StatusCodeHandlers = new[]
                {
                    typeof(JsonStatusHandler)
                };
            });

        protected override void ApplicationStartup(IContainer container, IPipelines pipelines)
        {
#if DEBUG
            StaticConfiguration.EnableRequestTracing = true;
#endif
            pipelines.OnError += (ctx, ex) =>
            {
                if (!(ex is Nancy.ModelBinding.ModelBindingException))
                {
                    return ctx.Response;
                }

                var serializer = new DefaultJsonSerializer();
                var error = new ErrorResponse
                {
                    Code = ApiCodes.MissingParameters,
                    Description = "Invalid Request Parameters",
                    Message = "Bad Request"
                };
                return new JsonResponse(error, serializer);
            };

            pipelines.EnableGzipCompression();
            base.ApplicationStartup(container, pipelines);
        }

        protected override IContainer GetApplicationContainer() => _container;
    }
}