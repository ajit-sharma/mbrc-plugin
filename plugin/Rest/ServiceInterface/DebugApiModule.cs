﻿#region Dependencies

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Nancy;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
	/// <summary>
	///     Service Responsible for Debug
	/// </summary>
	internal class DebugApiModule : NancyModule
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
		    Get["/debug"] = _ => new ResponseBase
		    {
		        Code = ApiCodes.Success
		    };

		    Get["/test"] = _ =>
		    {
		        this.module.UpdateArtistTable();
		        this.module.UpdateGenreTable();
		        this.module.UpdateAlbumTable();

		        return new ResponseBase
		        {
		            Code = ApiCodes.Success
		        };
		    };

		}
       
	}
}