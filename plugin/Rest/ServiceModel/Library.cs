#region

using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route(Routes.LibraryTracks, Verbs.Get, Summary = Summary.LibraryTracksGet)]
    public class GetLibraryTracks : IReturn<PaginatedTrackResponse>
    {
	    [ApiMember(Name = "limit", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false,
            Description = Description.Limit)]
        public int Limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false,
            Description = Description.Offset)]
        public int Offset { get; set; }
    }

    [Route(Routes.LibraryTracksId, Verbs.Get, Summary = Summary.LibraryTrackByIdGet)]
    public class GetLibraryTrack : IReturn<LibraryTrack>
    {
	    [ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true,
            Description = Description.EntryId)]
        public int Id { get; set; }
    }

    [Route(Routes.LibraryArtists, Verbs.Get, Summary = Summary.LibraryArtist)]
    public class GetLibraryArtists : IReturn<PaginatedArtistResponse>
    {
	    [ApiMember(Name = "limit", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Limit)]
        public int Limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Offset)]
        public int Offset { get; set; }
    }

    [Route(Routes.LibraryArtistsId, Verbs.Get, Summary = Summary.LibraryArtistById)]
    public class GetLibraryArtist : IReturn<LibraryArtist>
    {
	    [ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true, Description = Description.EntryId)]
        public int Id { get; set; }
    }

    [Route(Routes.LibraryAlbums, Verbs.Get, Summary = Summary.LibraryAlbums)]
    public class GetLibraryAlbums : IReturn<PaginatedAlbumResponse>
    {
	    [ApiMember(Name = "limit", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Limit)]
        public int Limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Offset)]
        public int Offset { get; set; }
    }

    [Route(Routes.LibraryAlbumsId, Verbs.Get, Summary = Summary.LibraryAlbumsId)]
    public class GetLibraryAlbum : IReturn<LibraryAlbum>
    {
	    [ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true, Description = Description.EntryId)]
        public int Id { get; set; }
    }

    [Route(Routes.LibraryGenres, Verbs.Get, Summary = Summary.LibraryGenres)]
    public class GetLibraryGenres : IReturn<PaginatedGenreResponse>
    {
	    [ApiMember(Name = "limit", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Limit)]
        public int Limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Offset)]
        public int Offset { get; set; }
    }

    [Route(Routes.LibraryGenresId, Verbs.Get, Summary = Summary.LibraryGenresId)]
    public class GetLibraryGenre : IReturn<LibraryGenre>
    {
	    [ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true, Description = Description.EntryId)]
        public int Id { get; set; }
    }

    [Route(Routes.LibraryCovers, Verbs.Get, Summary = Summary.LibraryCovers)]
    public class GetLibraryCovers : IReturn<PaginatedCoverResponse>
    {
	    [ApiMember(Name = "limit", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Limit)]
        public int Limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = SwaggerType.Int, IsRequired = false, Description = Description.Offset)]
        public int Offset { get; set; }
    }

    [Route(Routes.LibraryCoversId, Verbs.Get, Summary = Summary.LibraryCoversId)]
    public class GetLibraryCover : IReturn<LibraryCover>
    {
	    [ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true, Description = Description.EntryId)]
        public int Id { get; set; }
    }

    [Route(Routes.LibraryCoversIdRaw, Verbs.Get, Summary = Summary.LibraryCoversIdRaw)]
    public class GetLibraryCoverData
    {
	    [ApiMember(Name = "id", ParameterType = "path", DataType = SwaggerType.Int, IsRequired = true, Description = Description.EntryId)]
        public int Id { get; set; }
    }
}