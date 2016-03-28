using MusicBeeRemoteCore.Rest.ServiceInterface;
using MusicBeeRemoteCore.Rest.ServiceModel.Type;
using Nancy.Responses;

namespace MusicBeeRemoteCore
{
    using MusicBeeRemoteCore.Rest.Compression;
    using MusicBeeRemoteCore.Rest.StatusCodeHandlers;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Ninject;
    using Nancy.Diagnostics;
    using Ninject;

    class Bootstrapper : NinjectNancyBootstrapper
    {
        private IKernel container;

        public Bootstrapper(IKernel existingContainer)
        {
            this.container = existingContainer;
            this.container.Load<FactoryModule>();
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
            => new DiagnosticsConfiguration {Password = "12345"};

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return
                    NancyInternalConfiguration.WithOverrides(
                        configuration => { configuration.StatusCodeHandlers = new[] {typeof(JsonStatusHandler)}; });
            }
        }

        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
#if DEBUG
            StaticConfiguration.EnableRequestTracing = true;
#endif
            pipelines.OnError += (ctx, ex) =>
            {
                if (ex is Nancy.ModelBinding.ModelBindingException)
                {
                    var serializer = new DefaultJsonSerializer();
                    var error = new ErrorResponse
                    {
                        Code = ApiCodes.MissingParameters,
                        Description = "Invalid Request Parameters",
                        Message = "Bad Request"
                    };
                    return new JsonResponse(error, serializer);
                }

                return ctx.Response;
            };

            pipelines.EnableGzipCompression();
            base.ApplicationStartup(container, pipelines);
        }

        protected override IKernel GetApplicationContainer()
        {
            return this.container;
        }
    }
}