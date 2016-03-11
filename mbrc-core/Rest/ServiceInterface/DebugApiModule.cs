namespace MusicBeeRemoteCore.Rest.ServiceInterface
{
    using MusicBeeRemoteCore.Modules;
    using MusicBeeRemoteCore.Rest.ServiceModel.Type;

    using Nancy;

    /// <summary>
    ///     Service Responsible for Debug
    /// </summary>
    public class DebugApiModule : NancyModule
    {
        /// <summary>
        /// The module responsible for for the library API functionality.
        /// </summary>
        private readonly LibraryModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugApiModule"/> class. 
        /// </summary>
        /// <param name="module">
        /// The library module
        /// </param>
        public DebugApiModule(LibraryModule module)
            : base("/debug")
        {
            this.module = module;
            this.Get["/"] = _ => this.Response.AsJson(new ResponseBase { Code = ApiCodes.Success });

            this.Get["/update"] = _ =>
                {
                    this.module.UpdateArtistTable();
                    this.module.UpdateGenreTable();
                    this.module.UpdateAlbumTable();

                    return this.Response.AsJson(new ResponseBase { Code = ApiCodes.Success });
                };
        }
    }
}