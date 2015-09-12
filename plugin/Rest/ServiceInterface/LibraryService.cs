#region Dependencies

using MusicBeePlugin.Modules;
using MusicBeePlugin.Rest.ServiceModel;
using MusicBeePlugin.Rest.ServiceModel.Type;
using NServiceKit.Common.Web;
using NServiceKit.ServiceInterface;

#endregion

namespace MusicBeePlugin.Rest.ServiceInterface
{
	internal class LibraryService : Service
	{
		private readonly LibraryModule _module;

		public LibraryService(LibraryModule module)
		{
			_module = module;
		}

		public PaginatedResponse<LibraryTrack> Get(GetLibraryTracks request)
		{
			return _module.GetAllTracks(request.Limit, request.Offset, request.After);
		}

		public LibraryTrack Get(GetLibraryTrack request)
		{
			return _module.GetTrackById(request.Id);
		}

		public PaginatedResponse<LibraryArtist> Get(GetLibraryArtists request)
		{
			return _module.GetAllArtists(request.Limit, request.Offset, request.After);
		}

		public LibraryArtist Get(GetLibraryArtist request)
		{
			return _module.GetArtistById(request.Id);
		}

		public PaginatedResponse<LibraryGenre> Get(GetLibraryGenres request)
		{
			return _module.GetAllGenres(request.Limit, request.Offset, request.After);
		}

		public PaginatedResponse<LibraryAlbum> Get(GetLibraryAlbums request)
		{
			return _module.GetAllAlbums(request.Limit, request.Offset, request.After);
		}

		public PaginatedResponse<LibraryCover> Get(GetLibraryCovers request)
		{
			return _module.GetAllCovers(request.Offset, request.Limit, request.After);
		}

		public LibraryCover Get(GetLibraryCover request)
		{
			return _module.GetLibraryCover(request.Id, true);
		}

		[AddHeader(ContentType = "image/jpeg")]
		public object Get(GetLibraryCoverData request)
		{
			return new HttpResult(_module.GetCoverData(request.Id), "image/jpeg");
		}
	}
}