#region Dependencies

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using ServiceStack.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
	/// <summary>
	///     Service Responsible for Debug
	/// </summary>
	internal class DebugService : Service
	{
		private LibraryModule _module;

		/// <summary>
		/// </summary>
		/// <param name="module"></param>
		public DebugService(LibraryModule module)
		{
			_module = module;
		}

		public object Get(GetDebugLog request)
		{
			return "<pre>Demo</pre>";
		}

		public object Get(GetTest reqTest)
		{
			_module.UpdateArtistTable();
			_module.UpdateGenreTable();
			_module.UpdateAlbumTable();
			return new object {};
		}

	}
}