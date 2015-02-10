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
		private PlaylistModule _module;

		/// <summary>
		/// </summary>
		/// <param name="module"></param>
		public DebugService(PlaylistModule module)
		{
			_module = module;
		}

		public object Get(GetDebugLog request)
		{
			return "<pre>Demo</pre>";
		}

		public object Get(GetTest reqTest)
		{
			_module.SyncPlaylistsWithCache();
			return "per";
		}

	}
}