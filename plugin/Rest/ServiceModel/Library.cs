#region

using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
    [Route("/library/tracks", "GET", Summary = "Retrieves the library tracks stored in the database")]
    public class GetLibraryTracks : IReturn<PaginatedTrackResponse>
    {
	    [ApiMember(Name = "limit", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = Description.Limit)]
        public int limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = Description.Offset)]
        public int offset { get; set; }
    }

    [Route("/library/tracks/{id}", "GET", Summary = "Retrieves a track matching the specified id from the library")]
    public class GetLibraryTrack : IReturn<LibraryTrack>
    {
        [ApiMember(Name = "id", ParameterType = "path", DataType = "integer", IsRequired = true,
            Description = "The id of the entry to retrieve.")]
        public int id { get; set; }
    }

    [Route("/library/artists", "GET", Summary = "Retrieves the artists stored in the database")]
    public class GetLibraryArtists : IReturn<PaginatedArtistResponse>
    {
        [ApiMember(Name = "limit", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The number of results contained in the response.")]
        public int limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The position of the first entry in the response.")]
        public int offset { get; set; }
    }

    [Route("/library/artists/{id}", "GET", Summary = "Retrieves a single artist entry that matches the specified id.")]
    public class GetLibraryArtist : IReturn<LibraryArtist>
    {
        [ApiMember(Name = "id", ParameterType = "path", DataType = "integer", IsRequired = true,
            Description = "The id of the entry to retrieve.")]
        public int id { get; set; }
    }

    [Route("/library/albums", "GET", Summary = "Retrieves the albums stored in the database")]
    public class GetLibraryAlbums : IReturn<PaginatedAlbumResponse>
    {
        [ApiMember(Name = "limit", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The number of results contained in the response.")]
        public int limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The position of the first entry in the response.")]
        public int offset { get; set; }
    }

    [Route("/library/albums/{id}", "GET", Summary = "Retrieves a single album from the database.")]
    public class GetLibraryAlbum : IReturn<LibraryAlbum>
    {
        [ApiMember(Name = "id", ParameterType = "path", DataType = "integer", IsRequired = true,
            Description = "The id of the entry to retrieve.")]
        public int id { get; set; }
    }

    [Route("/library/genres", "GET", Summary = "Retrieves the genres stored in the database.")]
    public class GetLibraryGenres : IReturn<PaginatedGenreResponse>
    {
        [ApiMember(Name = "limit", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The number of results contained in the response.")]
        public int limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The position of the first entry in the response.")]
        public int offset { get; set; }
    }

    [Route("/library/genres/{id}", "GET", Summary = "Retrieves as single genre from the database.")]
    public class GetLibraryGenre : IReturn<LibraryGenre>
    {
        [ApiMember(Name = "id", ParameterType = "path", DataType = "integer", IsRequired = true,
            Description = "The id of the entry to retrieve.")]
        public int id { get; set; }
    }

    [Route("/library/covers", "GET", Summary = "Retrieves the cover entries stored in the database.")]
    public class GetLibraryCovers : IReturn<PaginatedCoverResponse>
    {
        [ApiMember(Name = "limit", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The number of results contained in the response.")]
        public int limit { get; set; }

        [ApiMember(Name = "offset", ParameterType = "query", DataType = "integer", IsRequired = false,
            Description = "The position of the first entry in the response.")]
        public int offset { get; set; }
    }

    [Route("/library/covers/{id}", "GET", Summary = "Retrieves a single cover from the database.")]
    public class GetLibraryCover : IReturn<LibraryCover>
    {
        [ApiMember(Name = "id", ParameterType = "path", DataType = "integer", IsRequired = true,
            Description = "The id of the entry to retrieve.")]
        public int id { get; set; }
    }

    [Route("/library/covers/{id}/raw", "GET", Summary = "Retrieves a jpeg cover image from the cache.")]
    public class GetLibraryCoverData
    {
        [ApiMember(Name = "id", ParameterType = "path", DataType = "integer", IsRequired = true,
            Description = "The id of the entry to retrieve.")]
        public int id { get; set; }
    }
}