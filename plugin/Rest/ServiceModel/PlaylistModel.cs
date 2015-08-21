#region Dependencies

using System.ComponentModel;
using System.Runtime.Serialization;
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

    [Route(Routes.Playlists, Verbs.Put, Summary = Summary.PlaylistPut)]
    [DataContract]
    public class CreatePlaylist : IReturn<SuccessResponse>
    {
        [DataMember(Name = "name", IsRequired = true)]
        [Description(Descriptions.PlaylistName)]
        public string Name { get; set; }

        [DataMember(Name = "list", IsRequired = false)]
        [Description(Descriptions.PlaylistList)]
        public string[] List { get; set; }
    }
   
    [Route(Routes.PlaylistsPlay, Verbs.Put, Summary = Summary.PlaylistPlay)]
    [DataContract]
    public class PlaylistPlay : IReturn<SuccessResponse>
    {
        [DataMember(Name = "path", IsRequired = true)]
        [Description(Descriptions.PlaylistPlay)]
        public string Path { get; set; }
    }

    [Route(Routes.PlaylistsId, Verbs.Get, Summary = Summary.GetsAPlaylist)]
    public class GetPlaylist : IdBasedRequest
    {
    }

    [Route(Routes.PlaylistsId, Verbs.Delete, Summary = Summary.DeletesAPlaylist)]
    public class DeletePlaylist : IdBasedRequest, IReturn<SuccessResponse>
    {
    }

    [Route(Routes.PlaylistsIdTracks, Verbs.Get, Summary = Summary.PlaylistTracks)]
    public class GetPlaylistTracks : PaginatedRequest, IReturn<PaginatedPlaylistTrackResponse>
    {
        [ApiMember(Name = "id", ParameterType = "path", IsRequired = true, DataType = SwaggerType.Int,
            Description = Descriptions.IdDesc)]
        public int Id { get; set; }
    }

    [Route(Routes.PlaylistsTrackInfo, Verbs.Get, Summary = Summary.PlaylistTrackInfo)]
    public class GetPlaylistTrackInfo : PaginatedRequest, IReturn<PaginatedPlaylistTrackInfoResponse>
    {
    }

    [Route(Routes.PlaylistsIdTracks, Verbs.Put, Summary = Summary.PlaylistTrackAdd)]
    [DataContract]
    public class AddPlaylistTracks : IdBasedRequest, IReturn<SuccessResponse>
    {
        [DataMember(Name = "list", IsRequired = true)]
        public string[] List { get; set; }
    }
   
    [Route(Routes.PlaylistsIdTracksMove, Verbs.Put, Summary = Summary.PlaylistTrackMove)]
    public class MovePlaylistTrack : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "id", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int,
            Description = Descriptions.PlaylistIdDesc)]
        public int Id { get; set; }

        [ApiMember(Name = "to", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int,
            Description = Descriptions.MoveTo)]
        public int To { get; set; }

        [ApiMember(Name = "from", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int,
            Description = Descriptions.MoveFrom)]
        public int From { get; set; }
    }

    [Route(Routes.PlaylistsIdTracksPosition, Verbs.Delete, Summary = Summary.PlaylistTrackDelete)]
    public class DeletePlaylistTracks : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "position", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int,
            Description = Descriptions.PlaylistTrackPosition)]
        public int Position { get; set; }

        [ApiMember(Name = "id", IsRequired = true, ParameterType = "path", DataType = SwaggerType.Int,
            Description = Descriptions.PlaylistIdDesc)]
        public int Id { get; set; }
    }

    [Route(Routes.PlaylistsChanges, Verbs.Get, Summary = Summary.PlaylistChanges)]
    public class GetPlaylistChanges : SyncRequest, IReturn<PaginatedPlaylistResponse>
    {
    }

    [Route(Routes.PlaylistTrackChanges, Verbs.Get, Summary = Summary.PlaylistTrackChanges)]
    public class GetPlaylistTrackChanges : SyncRequest, IReturn<PaginatedPlaylistTrackResponse>
    {
    }

    [Route(Routes.PlaylistTrackInfoChanges, Verbs.Get, Summary = Summary.PlaylistTrackInfoChanges)]
    public class GetPlaylistTrackInfoChanges : SyncRequest, IReturn<PaginatedPlaylistTrackInfoResponse>
    {
    }
}