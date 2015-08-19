#region Dependencies

using System;
using System.Runtime.Serialization;
using MusicBeePlugin.Rest.ServiceModel.Const;
using MusicBeePlugin.Rest.ServiceModel.Requests;
using MusicBeePlugin.Rest.ServiceModel.Type;
using ServiceStack.Api.Swagger;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

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
    [DataContract]
    public class CreatePlaylist : IReturn<SuccessResponse>
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "list", IsRequired = false)]
        public string[] List { get; set; }
    }

    [Api]
    [Route(Routes.PlaylistsPlay, Verbs.Put, Summary = Summary.PlaylistPlay)]
    public class PlaylistPlay : IReturn<SuccessResponse>
    {
        [ApiMember(Name = "path", IsRequired = true, Description = Description.PlaylistPlay,
            DataType = SwaggerType.String,
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
    [DataContract]
    public class AddPlaylistTracks : IdBasedRequest, IReturn<SuccessResponse>
    {
        [DataMember(Name = "list", IsRequired = true)]
        public string[] List { get; set; }
    }

    [Api]
    [Route(Routes.PlaylistsIdTracksMove, Verbs.Put, Summary = Summary.PlaylistTrackMove)]
    [DataContract]
    public class MovePlaylistTrack : IReturn<SuccessResponse>
    {
        [DataMember(Name = "id", IsRequired = true)]
        public int Id { get; set; }
    
        [DataMember(Name = "to", IsRequired = true)]
        public int To { get; set; }

        [DataMember(Name = "from", IsRequired = true)]
        public int From { get; set; }
    }

    [Route(Routes.PlaylistsIdTracks, Verbs.Delete, Summary = Summary.PlaylistTrackDelete)]
    public class DeletePlaylistTracks : IReturn<SuccessResponse>
    {
        public int position { get; set; }

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