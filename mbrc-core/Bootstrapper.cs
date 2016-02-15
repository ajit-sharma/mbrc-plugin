namespace MusicBeeRemoteCore
{
    using MusicBeePlugin.Rest.ServiceInterface;

    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Ninject;
    using Nancy.Diagnostics;

    using Ninject;

    class Bootstrapper : NinjectNancyBootstrapper
    {
        
        public Bootstrapper(IKernel existingContainer)
        {
            ConfigureApplicationContainer(existingContainer);
        }

        public bool RequestContainerConfigured { get; set; }
        public bool ApplicationContainerConfigured { get; set; }

        protected override void ConfigureApplicationContainer(IKernel existingContainer)
        {
            base.ConfigureApplicationContainer(existingContainer);
            this.ApplicationContainerConfigured = true;
        }
              
        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
#if DEBUG
            StaticConfiguration.EnableRequestTracing = true;
#endif
            base.ApplicationStartup(container, pipelines);
            
        }
        
        protected override void ConfigureRequestContainer(IKernel container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            this.RequestContainerConfigured = true;
        }

        

        protected override DiagnosticsConfiguration DiagnosticsConfiguration => new DiagnosticsConfiguration
                                                                                    {
                                                                                        Password = "12345"
                                                                                    };
    }
}