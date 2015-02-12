#region Dependencies

using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Requests;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
	[Route(Routes.LibraryTracks, Verbs.Get, Summary = Summary.LibraryTracksGet)]
	public class GetLibraryTracks : PaginatedRequest, IReturn<PaginatedTrackResponse>
	{
	}

	[Route(Routes.LibraryTracksU, Verbs.Get, Summary = Summary.LibrayTracksU)]
	public class GetLibraryTrackChanges : SyncRequest, IReturn<PaginatedTrackResponse>
	{
	}


	[Route(Routes.LibraryTracksId, Verbs.Get, Summary = Summary.LibraryTrackByIdGet)]
	public class GetLibraryTrack : IdBasedRequest, IReturn<LibraryTrack>
	{
	}

	[Route(Routes.LibraryArtists, Verbs.Get, Summary = Summary.LibraryArtist)]
	public class GetLibraryArtists : PaginatedRequest, IReturn<PaginatedArtistResponse>
	{
	}

	[Route(Routes.LibraryArtistsU, Verbs.Get, Summary = Summary.LibraryArtistU)]
	public class GetLibraryArtistChanges : SyncRequest, IReturn<PaginatedArtistResponse>
	{
	}

	[Route(Routes.LibraryArtistsId, Verbs.Get, Summary = Summary.LibraryArtistById)]
	public class GetLibraryArtist : IdBasedRequest, IReturn<LibraryArtist>
	{
	}

	[Route(Routes.LibraryAlbums, Verbs.Get, Summary = Summary.LibraryAlbums)]
	public class GetLibraryAlbums : PaginatedRequest, IReturn<PaginatedAlbumResponse>
	{
	}

	[Route(Routes.LibraryAlbumsU, Verbs.Get, Summary = Summary.LibraryAlbumsU)]
	public class GetLibraryAlbumChanges : SyncRequest, IReturn<PaginatedAlbumResponse>
	{
	}

	[Route(Routes.LibraryAlbumsId, Verbs.Get, Summary = Summary.LibraryAlbumsId)]
	public class GetLibraryAlbum : IdBasedRequest, IReturn<LibraryAlbum>
	{
	}

	[Route(Routes.LibraryGenres, Verbs.Get, Summary = Summary.LibraryGenres)]
	public class GetLibraryGenres : PaginatedRequest, IReturn<PaginatedGenreResponse>
	{
	}

	[Route(Routes.LibraryGenresU, Verbs.Get, Summary = Summary.LibraryGenresU)]
	public class GetLibraryGenreChanges : SyncRequest, IReturn<PaginatedGenreResponse>
	{
	}

	[Route(Routes.LibraryGenresId, Verbs.Get, Summary = Summary.LibraryGenresId)]
	public class GetLibraryGenre : IdBasedRequest, IReturn<LibraryGenre>
	{
	}

	[Route(Routes.LibraryCovers, Verbs.Get, Summary = Summary.LibraryCovers)]
	public class GetLibraryCovers : PaginatedRequest, IReturn<PaginatedCoverResponse>
	{
	}

	[Route(Routes.LibraryCoversU, Verbs.Get, Summary = Summary.LibraryCoversU)]
	public class GetLibraryCoverChanges : SyncRequest, IReturn<PaginatedCoverResponse>
	{
	}

	[Route(Routes.LibraryCoversId, Verbs.Get, Summary = Summary.LibraryCoversId)]
	public class GetLibraryCover : IdBasedRequest, IReturn<LibraryCover>
	{
	}

	[Route(Routes.LibraryCoversIdRaw, Verbs.Get, Summary = Summary.LibraryCoversIdRaw)]
	public class GetLibraryCoverData : IdBasedRequest
	{
	}
}
