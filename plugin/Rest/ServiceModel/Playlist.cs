#region

using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.ServiceHost;
using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Const;
using ServiceStack.Api.Swagger;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
	[Api("The API responsible for handling the playlist functionality.")]
    [Route(Routes.Playlists, Verbs.Get, Summary = Summary.PlaylistGet)]
    public class AllPlaylists : IReturn<PaginatedPlaylistResponse>
    {
	    [ApiMember(Name = "offset", IsRequired = false, DataType = SwaggerType.Int, Description = Description.Offset, ParameterType = "query")]
	    public int Offset { get; set; }
		[ApiMember(Name = "limit", IsRequired = false, DataType = SwaggerType.Int, Description = Description.Limit, ParameterType = "query")]
		public int Limit { get; set; }
    }

	[Api]
	[Route(Routes.Playlists, Verbs.Put, Summary = Summary.PlaylistPut)]
    public class CreatePlaylist : IReturn<SuccessResponse>
    {
	    [ApiMember(Name = "name", IsRequired = true, DataType = SwaggerType.String, Description = Description.PlaylistName, ParameterType = "query")]
	    public string Name { get; set; }
		[ApiMember(Name = "list", DataType = SwaggerType.Array, IsRequired = false, Description = Description.PlaylistList, ParameterType = "query")]
        public string[] List { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsPlay, Verbs.Put, Summary = Summary.PlaylistPlay)]
    public class PlaylistPlay : IReturn<SuccessResponse>
    {
	    [ApiMember(Name = "path", IsRequired = true, Description = Description.PlaylistPlay, DataType = SwaggerType.String, ParameterType = "query")]
        public string Path { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsId, Verbs.Get, Summary = Summary.GetsAPlaylist)]
    public class GetPlaylist
    {
		[ApiMember(Name = "id", ParameterType = "path", IsRequired = true, DataType = SwaggerType.Int, Description = Description.IdDesc)]
        public int Id { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsId, Verbs.Delete, Summary = Summary.DeletesAPlaylist)]
    public class DeletePlaylist : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "id", ParameterType = "path", IsRequired = true, DataType = SwaggerType.Int, Description = Description.IdDesc)]
		public int Id { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsIdTracks, Verbs.Get, Summary = Summary.PlaylistTracks)]
    public class GetPlaylistTracks : IReturn<PaginatedPlaylistTrackResponse>
    {
		[ApiMember(Name = "id", ParameterType = "path", IsRequired = true, DataType = SwaggerType.Int, Description = Description.IdDesc)]
		public int Id { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsIdTracks, Verbs.Put, Summary = Summary.PlaylistTrackAdd)]
    public class AddPlaylistTracks : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "list", DataType = SwaggerType.Array, IsRequired = false, Description = Description.PlaylistList, ParameterType = "query")]
		public string[] List { get; set; }

		[ApiMember(Name = "id", ParameterType = "path", IsRequired = true, DataType = SwaggerType.Int, Description = Description.IdDesc)]
		public int Id { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsIdTracksMove, Verbs.Put, Summary = Summary.PlaylistTrackMove)]
    public class MovePlaylistTrack : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "id", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int, Description = Description.PlaylistIdDesc)]
		public int Id { get; set; }
		[ApiMember(Name = "to", IsRequired = true,DataType = SwaggerType.Int, Description = Description.MoveTo, ParameterType = "query")]
        public int To { get; set; }
		[ApiMember(Name = "from", IsRequired = true,DataType = SwaggerType.Int, Description = Description.MoveFrom, ParameterType = "query")]
		public int From { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsIdTracks, Verbs.Delete, Summary = Summary.PlaylistTrackDelete)]
    public class DeletePlaylistTracks : IReturn<SuccessResponse>
    {
		[ApiMember(Name = "position", IsRequired = true, DataType = SwaggerType.Int, Description = Description.PlaylistTrackPosition, ParameterType = "query")]
        public int Position { get; set; }
		[ApiMember(Name = "id", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int, Description = Description.PlaylistIdDesc)]
		public int Id { get; set; }
    }

	[Api]
	[Route(Routes.PlaylistsUpdate, Verbs.Get, Summary = Summary.PlaylistUpdate)]
	public class GetPlaylistChanges
	{
	}

    [DataContract]
    public class SuccessResponse
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
    }
}