#region Dependencies

using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Requests;
using MusicBeePlugin.Rest.ServiceModel.Type;
using NServiceKit.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
	[Route(Routes.LibraryTracks, Verbs.Get, Summary = Summary.LibraryTracksGet)]
	public class GetLibraryTracks : PaginatedRequest, IReturn<PaginatedTrackResponse>
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

	[Route(Routes.LibraryArtistsId, Verbs.Get, Summary = Summary.LibraryArtistById)]
	public class GetLibraryArtist : IdBasedRequest, IReturn<LibraryArtist>
	{
	}

	[Route(Routes.LibraryAlbums, Verbs.Get, Summary = Summary.LibraryAlbums)]
	public class GetLibraryAlbums : PaginatedRequest, IReturn<PaginatedAlbumResponse>
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

	[Route(Routes.LibraryGenresId, Verbs.Get, Summary = Summary.LibraryGenresId)]
	public class GetLibraryGenre : IdBasedRequest, IReturn<LibraryGenre>
	{
	}

	[Route(Routes.LibraryCovers, Verbs.Get, Summary = Summary.LibraryCovers)]
	public class GetLibraryCovers : PaginatedRequest, IReturn<PaginatedCoverResponse>
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
