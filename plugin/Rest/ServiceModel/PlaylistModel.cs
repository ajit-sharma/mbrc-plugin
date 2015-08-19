#region Dependencies

using System;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Requests;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;

#endregion

namespace MusicBeePlugin.Rest.ServiceModel
{
	[Api("The API responsible for handling the playlist functionality.")]
	[Route(Routes.Playlists, Verbs.Get, Summary = Summary.PlaylistGet)]
	public class AllPlaylists : PaginatedRequest, IReturn<PaginatedPlaylistResponse>
	{
	}

	[Api]
	[Route(Routes.Playlists, Verbs.Put, Summary = Summary.PlaylistPut)]
	public class CreatePlaylist : IReturn<SuccessResponse>
	{
		[ApiMember(Name = "name", IsRequired = true, DataType = SwaggerType.String, Description = Description.PlaylistName,
			ParameterType = "body")]
		public string Name { get; set; }

		[ApiMember(Name = "list", DataType = SwaggerType.Array, IsRequired = false, Description = Description.PlaylistList,
			ParameterType = "body")]
		public string[] List { get; set; }
	}

	[Api]
	[Route(Routes.PlaylistsPlay, Verbs.Put, Summary = Summary.PlaylistPlay)]
	public class PlaylistPlay : IReturn<SuccessResponse>
	{
		[ApiMember(Name = "path", IsRequired = true, Description = Description.PlaylistPlay, DataType = SwaggerType.String,
			ParameterType = "body")]
		public string Path { get; set; }
	}

	[Api]
	[Route(Routes.PlaylistsId, Verbs.Get, Summary = Summary.GetsAPlaylist)]
	public class GetPlaylist : IdBasedRequest
	{
	}

	[Api]
	[Route(Routes.PlaylistsId, Verbs.Delete, Summary = Summary.DeletesAPlaylist)]
	public class DeletePlaylist : IdBasedRequest, IReturn<SuccessResponse>
	{
	}

	[Api]
	[Route(Routes.PlaylistsIdTracks, Verbs.Get, Summary = Summary.PlaylistTracks)]
	public class GetPlaylistTracks : PaginatedRequest, IReturn<PaginatedPlaylistTrackResponse>
	{
		[ApiMember(Name = "id", ParameterType = "path", IsRequired = true, DataType = SwaggerType.Int,
			Description = Description.IdDesc)]
		public int Id { get; set; }
	}

	[Api]
	[Route(Routes.PlaylistsTrackInfo, Verbs.Get, Summary = Summary.PlaylistTrackInfo)]
	public class GetPlaylistTrackInfo : PaginatedRequest, IReturn<PaginatedPlaylistTrackInfoResponse>
	{		
	}

	[Api]
	[Route(Routes.PlaylistsIdTracks, Verbs.Put, Summary = Summary.PlaylistTrackAdd)]
	public class AddPlaylistTracks : IdBasedRequest, IReturn<SuccessResponse>
	{
		[ApiMember(Name = "list", DataType = SwaggerType.Array, IsRequired = false, Description = Description.PlaylistList,
			ParameterType = "body")]
		public string[] List { get; set; }
	}

	[Api]
	[Route(Routes.PlaylistsIdTracksMove, Verbs.Put, Summary = Summary.PlaylistTrackMove)]
	public class MovePlaylistTrack : IReturn<SuccessResponse>
	{
		[ApiMember(Name = "id", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int,
			Description = Description.PlaylistIdDesc)]
		public int Id { get; set; }

		[ApiMember(Name = "to", IsRequired = true, DataType = SwaggerType.Int, Description = Description.MoveTo,
			ParameterType = "body")]
		public int To { get; set; }

		[ApiMember(Name = "from", IsRequired = true, DataType = SwaggerType.Int, Description = Description.MoveFrom,
			ParameterType = "body")]
		public int From { get; set; }
	}

	[Api]
	[Route(Routes.PlaylistsIdTracks, Verbs.Delete, Summary = Summary.PlaylistTrackDelete)]
	public class DeletePlaylistTracks : IReturn<SuccessResponse>
	{
		[ApiMember(Name = "position", IsRequired = true, DataType = SwaggerType.Int,
			Description = Description.PlaylistTrackPosition, ParameterType = "query")]
		public int Position { get; set; }

		[ApiMember(Name = "id", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int,
			Description = Description.PlaylistIdDesc)]
		public int Id { get; set; }
	}

	[Api]
	[Route(Routes.PlaylistsChanges, Verbs.Get, Summary = Summary.PlaylistChanges)]
	public class GetPlaylistChanges : SyncRequest, IReturn<PaginatedPlaylistResponse>
	{
	}

	[Api]
	[Route(Routes.PlaylistTrackChanges, Verbs.Get, Summary = Summary.PlaylistTrackChanges)]
	public class GetPlaylistTrackChanges : SyncRequest, IReturn<PaginatedPlaylistTrackResponse>
	{
	}

	[Api]
	[Route(Routes.PlaylistTrackInfoChanges, Verbs.Get, Summary = Summary.PlaylistTrackInfoChanges)]
	public class GetPlaylistTrackInfoChanges : SyncRequest, IReturn<PaginatedPlaylistTrackInfoResponse>
	{
	}
}
