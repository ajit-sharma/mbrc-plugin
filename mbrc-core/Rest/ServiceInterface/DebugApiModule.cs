namespace MusicBeePlugin.Rest.ServiceInterface
{
    using MusicBeePlugin.Modules;
    using MusicBeePlugin.Rest.ServiceModel.Type;

    using Nancy;

    /// <summary>
    ///     Service Responsible for Debug
    /// </summary>
    public class DebugApiModule : NancyModule
    {
        private readonly LibraryModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugApiModule"/> class. 
        /// </summary>
        /// <param name="module">
        /// </param>
        public DebugApiModule(LibraryModule module)
        {
            this.module = module;
            this.Get["/debug"] = _ => this.Response.AsJson(new ResponseBase { Code = ApiCodes.Success });

            this.Get["/test"] = _ =>
                {
                    this.module.UpdateArtistTable();
                    this.module.UpdateGenreTable();
                    this.module.UpdateAlbumTable();

                    return this.Response.AsJson(new ResponseBase { Code = ApiCodes.Success });
                };
        }
    }
}