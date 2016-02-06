#region Dependencies

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using Nancy;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
	/// <summary>
	///     Service Responsible for Debug
	/// </summary>
	internal class DebugApiModule : NancyModule
	{
		private readonly LibraryModule _module;

		/// <summary>
		/// </summary>
		/// <param name="module"></param>
		public DebugApiModule(LibraryModule module)
		{
			_module = module;
		    Get["/debug"] = _ => new ResponseBase
		    {
		        Code = ApiCodes.Success
		    };

		    Get["/test"] = _ =>
		    {
		        _module.UpdateArtistTable();
		        _module.UpdateGenreTable();
		        _module.UpdateAlbumTable();

		        return new ResponseBase
		        {
		            Code = ApiCodes.Success
		        };
		    };

		}
       
	}
}